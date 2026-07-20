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

    [SerializeField] private float muzzleFlashLifetime = 0.5f;

    private Queue<Rigidbody> _physicalAmmoPool;

    // Variables for recoil and spread control
    private float _progressiveSpread = 0f;
    private Vector3 _accumulatedRecoil = Vector3.zero;
    private bool _isFiringContinuously = false;

    private ObjectPool<GameObject> _muzzleFlashPool;

    public bool IsWeaponActive { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (GunData == null)
        {
            Debug.LogError($"Weapon {gameObject.name} is missing WeaponData!");
            return;
        }

        _muzzleFlashPool = ObjectPoolingManager.Instance.GetOrCreatePool(GunData.MuzzleFlashPrefab);
    }

    protected override void Update()
    {
        base.Update();
        _accumulatedRecoil = Vector3.Lerp(_accumulatedRecoil, Vector3.zero, Time.deltaTime * GunData.RecoilRecoverySpeed);
    }

    /*public override void HandleFireDown()
    {
        base.HandleFireDown();
        ResetSpread();
        //TryShoot();
    }

    public override void HandleFireHeld()
    {
        _isFiringContinuously = true;
        base.HandleFireHeld();
    }

    public override void HandleFireReleased()
    {
        _isFiringContinuously = false;
        ResetSpread();
        base.HandleFireReleased();
    }*/

    public override void HandleShoot()
    {
        SpawnProjectiles();
        SpawnMuzzleFlash();
        ApplyRecoil();
        ApplySpread();
        InvokeShoot();
    }

    // Spawn projectiles
    protected void SpawnProjectiles()
    {
        int bulletsPerShot = GetBulletsPerShot();

        // Spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 direction = GetShotDirectionWithinSpread(WeaponTip.forward);
            base.SpawnProjectile(direction);
        }
    }

    // Spawn muzzle flash effect
    private void SpawnMuzzleFlash()
    {
        if (GunData.MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = _muzzleFlashPool.Get();
            muzzleFlash.transform.position = WeaponTip.position;
            muzzleFlash.transform.SetParent(WeaponTip.transform);
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

    /*public void StartAiming()
    {
        Crosshair.DisableCrosshair();
        base.StartAiming();
    }

    public void StopAiming()
    {
        Crosshair.EnableCrosshair();
        base.StopAiming();
    }*/
}
