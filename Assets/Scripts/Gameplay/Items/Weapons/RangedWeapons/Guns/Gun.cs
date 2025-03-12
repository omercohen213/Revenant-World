using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Gun : RangedWeapon
{
    public GunData GunData => ItemData as GunData;

    public enum ShootType
    {
        Automatic,
        Burst,
        Single
    }

    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform WeaponMuzzle;

    [Header("Audio & Visual")]
    [SerializeField] private float muzzleFlashLifetime = 0.5f;

    private float _lastTimeShot = Mathf.NegativeInfinity;
    private Queue<Rigidbody> _physicalAmmoPool;

    // Variables for recoil and spread control
    private float _progressiveSpread = 0f;
    private Vector3 _accumulatedRecoil = Vector3.zero;
    private bool _isFiringContinuously = false;

    // Object pools
    public ObjectPool<Bullet> BulletPool;
    private ObjectPool<GameObject> _muzzleFlashPool;

    public bool IsWeaponActive { get; private set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (GunData == null)
        {
            Debug.LogError($"Weapon {gameObject.name} is missing WeaponData!");
            return;
        }

        CurrentAmmo = 0;

        // initialize object pools
        BulletPool = ObjectPoolingManager.Instance.GetOrCreatePool(GunData.BulletPrefab);
        _muzzleFlashPool = ObjectPoolingManager.Instance.GetOrCreatePool(GunData.MuzzleFlashPrefab);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Owner = GetComponentInParent<Player>();
    }

    protected override void Update()
    {
        base.Update();
        _accumulatedRecoil = Vector3.Lerp(_accumulatedRecoil, Vector3.zero, Time.deltaTime * GunData.RecoilRecoverySpeed);
    }

    public override void HandleFireDown()
    {
        base.HandleFireDown();
        ResetSpread();
        TryShoot();
    }

    public override void HandleFireHeld()
    {
        base.HandleFireHeld();
        _isFiringContinuously = true;
        TryShoot();
    }

    public override void HandleFireReleased()
    {
        base.HandleFireReleased();
        _isFiringContinuously = false;
        ResetSpread();
    }

    public override void Reload(int ammoToReload)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo + ammoToReload, GunData.ClipSize);
        InvokeReload();
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

    public override void HandleShoot()
    {
        _lastTimeShot = Time.time;
        SpawnBullet();
        SpawnMuzzleFlash();
        ApplyRecoil();
        ApplySpread();
        InvokeShoot();
    }

    // Spawn a bullet
    private void SpawnBullet()
    {
        int bulletsPerShot = GetBulletsPerShot();

        // Spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle.forward);

            // Spawn bullet with calculated spread direction
            Bullet newBullet = BulletPool.Get();
            newBullet.transform.position = WeaponMuzzle.position;
            newBullet.transform.rotation = Quaternion.LookRotation(shotDirection);

            // Call Shoot to apply initial properties or velocity; this resets the bullet as needed
            newBullet.Shoot(this);
        }
    }

    // Spawn muzzle flash effect
    private void SpawnMuzzleFlash()
    {
        if (GunData.MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = _muzzleFlashPool.Get();
            muzzleFlash.transform.position = WeaponMuzzle.position;
            muzzleFlash.transform.SetParent(WeaponMuzzle.transform);
            StartCoroutine(SelfDestructCoroutine(muzzleFlash, muzzleFlashLifetime));
        }
    }

    private IEnumerator SelfDestructCoroutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        _muzzleFlashPool.Release(obj);
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
}
