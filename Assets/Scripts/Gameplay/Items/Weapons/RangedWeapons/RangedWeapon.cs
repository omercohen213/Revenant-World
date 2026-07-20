using System;
using System.Collections;
using UnityEngine;

public abstract class RangedWeapon : Weapon, IRangedWeapon
{
    [HideInInspector] public RangedWeaponData RangedWeaponData => ItemData as RangedWeaponData;

    public event Action OnShoot;
    public event Action OnReload;

    public int CurrentAmmo { get; set; }

    protected bool _isReloading = false;
    public bool IsReloading => _isReloading;

    private float _lastTimeShot = Mathf.NegativeInfinity;
    private Coroutine _reloadCoroutine;

    protected void InvokeShoot() => OnShoot?.Invoke();

    protected virtual void OnEnable()
    {
        CurrentAmmo = RangedWeaponData.HasInfiniteAmmo ? int.MaxValue : 0;
    }

    protected override void Update()
    {
        //CheckReloading();
        CheckAutomaticReload();
    }

    /*// Prevent actions while reloading
    private void CheckReloading()
    {
        if (_isReloading)
        {
            if (_isAiming)
            {
                StopAiming();
            }
            return;
        }
    }*/

    // Reload automatically if no ammo left
    private void CheckAutomaticReload()
    {
        if (CurrentAmmo <= 0)
        {
            StartReloading();
            return;
        }
    }
    
    public override void TryAttack()
    {
        TryShoot();  
    }

    private void TryShoot()
    {
        if (CurrentAmmo > 0 || RangedWeaponData.HasInfiniteAmmo)
        {
            if (Time.time - _lastTimeShot >= RangedWeaponData.DelayBetweenShots)
            {
                _lastTimeShot = Time.time;
                CurrentAmmo--;
                HandleShoot();
            }
        }
    }

    public virtual void HandleShoot()
    {
        _lastTimeShot = Time.time;
        SpawnProjectile(WeaponTip.forward);
        OnShoot?.Invoke();
    }

    public virtual void Reload(int ammoToReload)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo + ammoToReload, RangedWeaponData.ClipSize);
        OnReload?.Invoke();
    }

    public void StartReloadAnimation()
    {
        //GetComponent<Animator>().SetTrigger("Reload");
    }

    public void StopReloadAnimation()
    {
        //GetComponent<Animator>().SetTrigger("Idle");
    }

    // Check if reload is allowed
    public bool CanReload()
    {
        if (RangedWeaponData.HasInfiniteAmmo)
        {
            return false;
        }

        int totalAmmo = _inventoryManager.GetTotalQuantityOfItem(RangedWeaponData.RequiredAmmo);
        int ammoNeeded = RangedWeaponData.ClipSize - CurrentAmmo;
        return !_isReloading && totalAmmo > 0 && ammoNeeded > 0 || CurrentAmmo <= 0 && totalAmmo > 0;
    }


    // Handle the start of a reload
    public void StartReloading()
    {
        if (_reloadCoroutine != null) return; // Prevent multiple reloads at once

        // Stop aiming
        /*if (_isAiming)
        {
            StopAiming();
            _isAiming = false;
        }*/

        _isReloading = true;
        _reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }

    // Handle the reloading process over time. Is only for ranged weapons
    private IEnumerator ReloadCoroutine()
    {
        StartReloadAnimation();

        float reloadTime = RangedWeaponData.ReloadTime;
        yield return new WaitForSeconds(reloadTime);


        int ammoNeeded = RangedWeaponData.ClipSize - CurrentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, _inventoryManager.GetTotalQuantityOfItem(RangedWeaponData.RequiredAmmo));

        _inventoryManager.ReduceItemQuantity(RangedWeaponData.RequiredAmmo, ammoToReload);
        Reload(ammoToReload);

        StopReloading();
    }

    // Properly stops the reloading process
    private void StopReloading()
    {
        _isReloading = false;
        _reloadCoroutine = null;
        StopReloadAnimation();
    }
   
}
