using DevionGames.InventorySystem;
using GLTF.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public enum ShootType
    {
        Automatic,
        Burst,
        Single
    }

    [Header("Weapon Data")]
    [Tooltip("Reference to the ScriptableObject holding weapon stats")]
    public WeaponData WeaponData;

    [Header("Information")]
    public Entity Owner;

    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject WeaponRoot;

    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform WeaponMuzzle;

    [Header("Scope Settings")]
    [Tooltip("The exact position where the aim camera should move to align with the scope")]
    public Transform ScopeTransform;

    [Header("Audio & Visual")]

    [Tooltip("Prefab of the muzzle flash")]
    public GameObject MuzzleFlashPrefab;

    [Tooltip("Unparent the muzzle flash instance on spawn")]
    public bool UnparentMuzzleFlash;

    public event Action OnShoot;
    public event Action OnReload;

    public int CurrentAmmo;

    private float _lastTimeShot = Mathf.NegativeInfinity;
    private Vector3 _lastMuzzlePosition;
    private Queue<Rigidbody> _physicalAmmoPool;

    // Variables for recoil and spread control
    private float _progressiveSpread = 0f;
    private Vector3 _accumulatedRecoil = Vector3.zero;
    private bool _isFiringContinuously = false;

    public bool IsWeaponActive { get; private set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }

    void Awake()
    {
        if (WeaponData == null)
        {
            Debug.LogError($"Weapon {gameObject.name} is missing WeaponData!");
            return;
        }

        CurrentAmmo = WeaponData.ClipSize;
        _lastMuzzlePosition = WeaponMuzzle.position;
        Owner = DebugUtil.GetFirstParentOfType<Player>(gameObject);
    }

    private void Update()
    {
        //_accumulatedRecoil = Vector3.Lerp(_accumulatedRecoil, Vector3.zero, Time.deltaTime * WeaponData.RecoilRecoverySpeed);
        _accumulatedRecoil = Vector3.Lerp(_accumulatedRecoil, Vector3.zero, Time.deltaTime * WeaponData.RecoilRecoverySpeed);
    }

    public void StartShooting()
    {
        _progressiveSpread = 0f; // Reset spread
        _isFiringContinuously = true;
        TryShoot();
    }

    public void ContinueShooting()
    {
        _isFiringContinuously = true;
    }

    public void StopShooting()
    {
        //_isFiringContinuously = false;
        //ResetSpread();
    }

    public void Reload(int ammoToReload)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo + ammoToReload, WeaponData.ClipSize);
        OnReload?.Invoke();
    }

    public void StartReloadAnimation()
    {
       //GetComponent<Animator>().SetTrigger("Reload");
    }

    public void StopReloadAnimation()
    {
        //GetComponent<Animator>().enabled = false;
    }

    bool TryShoot()
    {
        if (WeaponData == null)
        {
            Debug.LogError("WeaponData is missing!");
            return false;
        }

        if (CurrentAmmo > 0 && Time.time - _lastTimeShot >= WeaponData.DelayBetweenShots)
        {
            _lastTimeShot = Time.time;
            CurrentAmmo--;
            HandleShoot();
            return true;
        }
        return false;
    }

    private void HandleShoot()
    {
        int bulletsPerShot = GetBulletsPerShot();

        // Spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.forward);

            // Instantiate bullet with calculated spread direction
            Bullet newBullet = Instantiate(WeaponData.BulletPrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
            newBullet.Shoot(this);
        }

        HandleMuzzleFlash();
        _lastTimeShot = Time.time;

        ApplyRecoil();
        ApplySpread();

        OnShoot?.Invoke();
    }

    private int GetBulletsPerShot()
    {
        return WeaponData.ShootType switch
        {
            ShootType.Automatic or ShootType.Single => 1,
            ShootType.Burst => 3,
            _ => 1,
        };
    }

    public void ApplyRecoil()
    {
        if (WeaponData.ShootType == ShootType.Automatic)
        {
            _accumulatedRecoil += Vector3.back * WeaponData.RecoilForce;
            _accumulatedRecoil = Vector3.ClampMagnitude(_accumulatedRecoil, WeaponData.MaxRecoilDistance);
        }
        //WeaponRoot.transform.localPosition += _accumulatedRecoil;
    }

    // Calculates bullet spread based on weapon type and continuous fire
    private Vector3 GetShotDirectionWithinSpread(Vector3 baseDirection)
    {
        float spread = WeaponData.BaseSpread;

        // Increase spread progressively for automatic weapons
        if (WeaponData.ShootType == ShootType.Automatic && _isFiringContinuously)
        {
            spread += _progressiveSpread;
            _progressiveSpread = Mathf.Min(_progressiveSpread + WeaponData.SpreadIncreasePerShot, WeaponData.MaxSpread);
        }
        else
        {
            _progressiveSpread = 0f; // Reset spread for single-shot weapons
        }

        // Apply random spread offset
        Vector3 randomOffset = Random.insideUnitSphere * spread;
        //Debug.Log($"Spread: {spread}, Direction: {randomOffset}"); // Debug log
        return (baseDirection + randomOffset).normalized;
    }

    public void ApplySpread()
    {
        // Increase spread per shot
        _progressiveSpread += WeaponData.SpreadIncreasePerShot;
        _progressiveSpread = Mathf.Min(_progressiveSpread, WeaponData.MaxSpread);
    }

    public void ResetSpread()
    {
        _progressiveSpread = WeaponData.BaseSpread;

    }

    // Handles muzzle flash effect
    private void HandleMuzzleFlash()
    {
        if (MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);
            if (UnparentMuzzleFlash)
            {
                muzzleFlashInstance.transform.SetParent(null);
            }
            Destroy(muzzleFlashInstance, 2f);
        }
    }


}
