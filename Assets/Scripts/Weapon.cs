using System;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public enum WeaponShootType
    {
        Burst,
        Automatic,
        Single
    }

    [Header("Information")]
    [Tooltip("The name that will be displayed in the UI for this weapon")]
    public string WeaponName;

    [Tooltip("The image that will be displayed in the UI for this weapon")]
    public Sprite WeaponIcon;

    [Header("Internal References")]
    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject WeaponRoot;

    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform WeaponMuzzle;

    [Header("Shoot Parameters")]
    [Tooltip("The type of weapon will affect how it shoots")]
    public WeaponShootType ShootType;

    [Tooltip("The bullet prefab")] public Bullet BulletPrefab;

    [Tooltip("Minimum duration between two shots")]
    public float DelayBetweenShots = 0.5f;

    [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
    public float BulletSpreadAngle = 0f;

    [Tooltip("Amount of bullets per shot")]
    public int BulletsPerShot = 1;

    [Tooltip("Force that will push back the weapon after each shot")]
    [Range(0f, 2f)]
    public float RecoilForce = 1;

    [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
    [Range(0f, 1f)]
    public float AimZoomRatio = 1f;

    [Header("Scope Settings")]
    [Tooltip("The exact position where the aim camera should move to align with the scope")]
    public Transform ScopeTransform;

    [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
    public Vector3 AimOffset;

    [Header("Ammo Parameters")]
    [Tooltip("Should the player manually reload")]
    public bool AutomaticReload = true;
    [Tooltip("Has physical clip on the weapon and ammo shells are ejected when firing")]
    public bool HasPhysicalBullets = false;
    [Tooltip("Number of bullets in a clip")]
    public int ClipSize = 30;
    [Tooltip("Bullet Shell Casing")]
    public GameObject ShellCasing;
    [Tooltip("Weapon Ejection Port for physical ammo")]
    public Transform EjectionPort;
    [Tooltip("Force applied on the shell")]
    [Range(0.0f, 5.0f)] public float ShellCasingEjectionForce = 2.0f;
    [Tooltip("Maximum number of shell that can be spawned before reuse")]
    [Range(1, 30)] public int ShellPoolSize = 1;
    [Tooltip("Amount of ammo reloaded per second")]
    public float AmmoReloadRate = 1f;

    [Tooltip("Delay after the last shot before starting to reload")]
    public float AmmoReloadDelay = 2f;

    [Tooltip("Maximum amount of ammo in the gun")]
    public int MaxAmmo = 30;

    [Header("Audio & Visual")]
    [Tooltip("Optional weapon animator for OnShoot animations")]
    public Animator WeaponAnimator;

    [Tooltip("Prefab of the muzzle flash")]
    public GameObject MuzzleFlashPrefab;

    [Tooltip("Unparent the muzzle flash instance on spawn")]
    public bool UnparentMuzzleFlash;

    public UnityAction OnShoot;
    public event Action OnShootProcessed;

    private float m_CurrentAmmo;
    private float m_LastTimeShot = Mathf.NegativeInfinity;
    public float LastChargeTriggerTimestamp { get; private set; }
    private Vector3 m_LastMuzzlePosition;

    public GameObject Owner;
    public GameObject SourcePrefab { get; set; }
    public float CurrentAmmoRatio { get; private set; }
    public bool IsWeaponActive { get; private set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }
    public int GetCurrentAmmo() => Mathf.FloorToInt(m_CurrentAmmo);

    public bool IsReloading { get; private set; }

    const string k_AnimAttackParameter = "Attack";

    private Queue<Rigidbody> m_PhysicalAmmoPool;

    void Awake()
    {
        m_CurrentAmmo = MaxAmmo;
        m_LastMuzzlePosition = WeaponMuzzle.position;
    }

    void Update()
    {
        UpdateAmmo();
        if (Time.deltaTime > 0)
        {
            MuzzleWorldVelocity = (WeaponMuzzle.position - m_LastMuzzlePosition) / Time.deltaTime;
            m_LastMuzzlePosition = WeaponMuzzle.position;
        }
    }

    void UpdateAmmo()
    {
        if (AutomaticReload && m_LastTimeShot + AmmoReloadDelay < Time.time && m_CurrentAmmo < MaxAmmo)
        {
            // reloads weapon over time
            m_CurrentAmmo += AmmoReloadRate * Time.deltaTime;

            // limits ammo to max value
            m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo, 0, MaxAmmo);
        }

        /* if (MaxAmmo == Mathf.Infinity)
         {
             CurrentAmmoRatio = 1f;
         }
         else
         {
             CurrentAmmoRatio = m_CurrentAmmo / MaxAmmo;
         }*/
    }

    void Reload()
    {
        m_CurrentAmmo = ClipSize;
        IsReloading = false;
    }

    public void StartReloadAnimation()
    {
        if (m_CurrentAmmo < ClipSize)
        {
            //GetComponent<Animator>().SetTrigger("Reload");
            IsReloading = true;
        }
    }

    public void UseAmmo(float amount)
    {
        m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo - amount, 0f, MaxAmmo);
        m_LastTimeShot = Time.time;
    }

    public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
    {
        switch (ShootType)
        {
            case WeaponShootType.Burst:
            case WeaponShootType.Single:
                if (inputDown)
                {
                    return TryShoot();
                }

                return false;

            case WeaponShootType.Automatic:
                if (inputHeld)
                {
                    return TryShoot();
                }

                return false;

            default:
                return false;
        }
    }

    bool TryShoot()
    {
        if (m_CurrentAmmo >= 1f
            && m_LastTimeShot + DelayBetweenShots < Time.time)
        {
            HandleShoot();
            m_CurrentAmmo -= 1f;

            return true;
        }

        return false;
    }

    void HandleShoot()
    {
        BulletsPerShot = GetBulletsPerShot();

        // spawn all bullets with random direction
        for (int i = 0; i < BulletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
            Bullet newBullet = Instantiate(BulletPrefab, WeaponMuzzle.position,
                Quaternion.LookRotation(shotDirection));
            newBullet.Shoot(this);
        }

        // muzzle flash
        if (MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position,
                WeaponMuzzle.rotation, WeaponMuzzle.transform);
            // Unparent the muzzleFlashInstance
            if (UnparentMuzzleFlash)
            {
                muzzleFlashInstance.transform.SetParent(null);
            }

            Destroy(muzzleFlashInstance, 2f);
        }

        /*if (HasPhysicalBullets)
        {
            ShootShell();
            m_CarriedPhysicalBullets--;
        }*/

        m_LastTimeShot = Time.time;

        // Trigger attack animation if there is any
        if (WeaponAnimator)
        {
            WeaponAnimator.SetTrigger(k_AnimAttackParameter);
        }

        OnShoot?.Invoke();
        OnShootProcessed?.Invoke();
    }

    private int GetBulletsPerShot()
    {
        return ShootType switch
        {
            WeaponShootType.Automatic or WeaponShootType.Single => 1,
            WeaponShootType.Burst => 3,
            _ => 1,
        };
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);

        return spreadWorldDirection;
    }
}
