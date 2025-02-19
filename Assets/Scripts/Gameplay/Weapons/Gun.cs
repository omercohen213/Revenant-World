using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : RangedWeapon
{
    public enum ShootType
    {
        Automatic,
        Burst,
        Single
    }

    [Header("Weapon Data")]
    [Tooltip("Reference to the ScriptableObject holding gun stats")]
    public GunData GunData;

    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform WeaponMuzzle;

    [Header("Audio & Visual")]
    [Tooltip("Prefab of the muzzle flash")]
    public GameObject MuzzleFlashPrefab;

    [Tooltip("Unparent the muzzle flash instance on spawn")]
    public bool UnparentMuzzleFlash;

    private float _lastTimeShot = Mathf.NegativeInfinity;
    private Queue<Rigidbody> _physicalAmmoPool;

    // Variables for recoil and spread control
    private float _progressiveSpread = 0f;
    private Vector3 _accumulatedRecoil = Vector3.zero;
    private bool _isFiringContinuously = false;

    public bool IsWeaponActive { get; private set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }

    void Awake()
    {
        if (GunData == null)
        {
            Debug.LogError($"Weapon {gameObject.name} is missing WeaponData!");
            return;
        }

        CurrentAmmo = GunData.ClipSize;
        Owner = DebugUtil.GetFirstParentOfType<Player>(gameObject);
    }

    private void Update()
    {
        //_accumulatedRecoil = Vector3.Lerp(_accumulatedRecoil, Vector3.zero, Time.deltaTime * WeaponData.RecoilRecoverySpeed);
        _accumulatedRecoil = Vector3.Lerp(_accumulatedRecoil, Vector3.zero, Time.deltaTime * GunData.RecoilRecoverySpeed);
    }

    public override void StartShooting()
    {
        _progressiveSpread = 0f; // Reset spread
        _isFiringContinuously = true;
        TryShoot();
    }

    public override void ContinueShooting()
    {
        _isFiringContinuously = true;
    }

    public override void StopShooting()
    {
        //_isFiringContinuously = false;
        //ResetSpread();
    }

    public override void Reload(int ammoToReload)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo + ammoToReload, GunData.ClipSize);
        InvokeReload();
    }

    public override void StartReloadAnimation()
    {
        //GetComponent<Animator>().SetTrigger("Reload");
    }

    public override void StopReloadAnimation()
    {
        //GetComponent<Animator>().enabled = false;
    }

    public override bool TryShoot()
    {
        if (GunData == null)
        {
            Debug.LogError("WeaponData is missing!");
            return false;
        }

        if (CurrentAmmo > 0 && Time.time - _lastTimeShot >= GunData.DelayBetweenShots)
        {
            _lastTimeShot = Time.time;
            CurrentAmmo--;
            HandleShoot();
            return true;
        }
        return false;
    }

    protected override void HandleShoot()
    {
        int bulletsPerShot = GetBulletsPerShot();

        // Spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.forward);

            // Instantiate bullet with calculated spread direction
            Bullet newBullet = Instantiate(GunData.BulletPrefab, WeaponMuzzle.position, Quaternion.LookRotation(shotDirection));
            newBullet.Shoot(this);
        }

        HandleMuzzleFlash();
        _lastTimeShot = Time.time;

        ApplyRecoil();
        ApplySpread();

        InvokeShoot();
    }

    private int GetBulletsPerShot()
    {
        return GunData.ShootType switch
        {
            ShootType.Automatic or ShootType.Single => 1,
            ShootType.Burst => 3,
            _ => 1,
        };
    }

    public void ApplyRecoil()
    {
        if (GunData.ShootType == ShootType.Automatic)
        {
            _accumulatedRecoil += Vector3.back * GunData.RecoilForce;
            _accumulatedRecoil = Vector3.ClampMagnitude(_accumulatedRecoil, GunData.MaxRecoilDistance);
        }
        //WeaponRoot.transform.localPosition += _accumulatedRecoil;
    }

    // Calculates bullet spread based on weapon type and continuous fire
    private Vector3 GetShotDirectionWithinSpread(Vector3 baseDirection)
    {
        float spread = GunData.BaseSpread;

        // Increase spread progressively for automatic weapons
        if (GunData.ShootType == ShootType.Automatic && _isFiringContinuously)
        {
            spread += _progressiveSpread;
            _progressiveSpread = Mathf.Min(_progressiveSpread + GunData.SpreadIncreasePerShot, GunData.MaxSpread);
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
        _progressiveSpread += GunData.SpreadIncreasePerShot;
        _progressiveSpread = Mathf.Min(_progressiveSpread, GunData.MaxSpread);
    }

    public void ResetSpread()
    {
        _progressiveSpread = GunData.BaseSpread;

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
