using System;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public enum WeaponShootType
    {
        Manual,
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

    [Tooltip("The projectile prefab")] public ProjectileBase ProjectilePrefab;

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

    [Tooltip("sound played when shooting")]
    public AudioClip ShootSfx;

    [Tooltip("Sound played when changing to this weapon")]
    public AudioClip ChangeWeaponSfx;

    [Tooltip("Continuous Shooting Sound")] public bool UseContinuousShootSound = false;
    public AudioClip ContinuousShootStartSfx;
    public AudioClip ContinuousShootLoopSfx;
    public AudioClip ContinuousShootEndSfx;
    private AudioSource _continuousShootAudioSource = null;
    private bool _wantsToShoot = false;

    public UnityAction OnShoot;
    public event Action OnShootProcessed;

    private int _carriedPhysicalBullets;
    private float m_CurrentAmmo;
    private float m_LastTimeShot = Mathf.NegativeInfinity;
    public float LastChargeTriggerTimestamp { get; private set; }
    private Vector3 _lastMuzzlePosition;

    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsCharging { get; private set; }
    public float CurrentAmmoRatio { get; private set; }
    public bool IsWeaponActive { get; private set; }
    public bool IsCooling { get; private set; }
    public float CurrentCharge { get; private set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }
    public int GetCurrentAmmo() => Mathf.FloorToInt(m_CurrentAmmo);

    private AudioSource _shootAudioSource;

    public bool IsReloading { get; private set; }

    const string k_AnimAttackParameter = "Attack";

    private Queue<Rigidbody> m_PhysicalAmmoPool;

    void Awake()
    {
        m_CurrentAmmo = MaxAmmo;
        _carriedPhysicalBullets = HasPhysicalBullets ? ClipSize : 0;
        _lastMuzzlePosition = WeaponMuzzle.position;

        _shootAudioSource = GetComponent<AudioSource>();
        DebugUtility.HandleErrorIfNullGetComponent<AudioSource, WeaponController>(_shootAudioSource, this,
            gameObject);

        if (UseContinuousShootSound)
        {
            _continuousShootAudioSource = gameObject.AddComponent<AudioSource>();
            _continuousShootAudioSource.playOnAwake = false;
            _continuousShootAudioSource.clip = ContinuousShootLoopSfx;
            _continuousShootAudioSource.outputAudioMixerGroup =
                AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.WeaponShoot);
            _continuousShootAudioSource.loop = true;
        }

        if (HasPhysicalBullets)
        {
            m_PhysicalAmmoPool = new Queue<Rigidbody>(ShellPoolSize);

            for (int i = 0; i < ShellPoolSize; i++)
            {
                GameObject shell = Instantiate(ShellCasing, transform);
                shell.SetActive(false);
                m_PhysicalAmmoPool.Enqueue(shell.GetComponent<Rigidbody>());
            }
        }
    }
}
