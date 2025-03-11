using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RangedWeapon : Weapon, IRangedWeapon
{
    [HideInInspector] public RangedWeaponData RangedWeaponData;

    public event Action OnShoot;
    public event Action OnReload;

    public bool IsAiming { get; private set; }
    public int CurrentAmmo { get; set; }

    private bool _canAim = true;
    protected float _weaponFovMultiplier = 0.5f; //change to scope
    private readonly float _aimingAnimDuration = 0.3f;

    private Coroutine _reloadCoroutine;
    private Coroutine _aimCoroutine;

    protected void InvokeShoot() => OnShoot?.Invoke();
    protected void InvokeReload() => OnReload?.Invoke();

    // Main method to handle weapon actions
    public override void HandleActions()
    {
        // Prevent actions while reloading
        if (IsReloading)
        {
            if (IsAiming)
            {
                StopAiming();
            }
            return;
        }

        // Reload automatically if no ammo left
        if (CurrentAmmo <= 0)
        {
            StartReloading();
            return;
        }
        CheckInput();
    }

    // Check for player input
    private void CheckInput()
    {
        // Fire input
        if (_playerInput.GetFireInputDown())
        {
            StartShooting();
        }

        if (_playerInput.GetFireInputHeld())
        {
            ContinueShooting();
        }

        if (_playerInput.GetFireInputReleased())
        {
            StopShooting();
        }

        // Reload input
        if (CanReload() && _playerInput.Reload)
        {
            StartReloading();
        }

        // Aim input
        if (_canAim && _playerInput.GetAimInputDown())
        {
            if (!IsAiming)
            {
                StartAiming();
            }
            else
            {
                StopAiming();
            }
        }
    }

    public virtual void StartShooting()
    {

    }

    public virtual void ContinueShooting()
    {

    }

    public virtual void StopShooting()
    {

    }

    public virtual bool TryShoot()
    {
        return false;
    }

    public virtual void HandleShoot()
    {

    }
    public virtual void Reload(int ammoToReload)
    {

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
    private bool CanReload()
    {
        if (RangedWeaponData.HasAmmo)
        {
            int totalAmmo = _inventoryManager.GetTotalQuantityOfItem(RangedWeaponData.RequiredAmmo);
            int ammoNeeded = RangedWeaponData.ClipSize - CurrentAmmo;
            return !IsReloading && totalAmmo > 0 && ammoNeeded > 0 || CurrentAmmo <= 0 && totalAmmo > 0;
        }
        else return false;
    }

    // Handle the start of a reload
    private void StartReloading()
    {
        if (_reloadCoroutine != null) return; // Prevent multiple reloads at once

        // Stop aiming
        if (IsAiming)
        {
            StopAiming();
            IsAiming = false;
        }

        IsReloading = true;
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
        IsReloading = false;
        _reloadCoroutine = null;
        StopReloadAnimation();
    }

    private void UpdateFov()
    {
        float targetFov = IsAiming ? _weaponFovMultiplier * _defaultFov : _defaultFov;
        _cameraManager.StartFovTransition(targetFov, _aimingAnimDuration);
    }

    public virtual void StartAiming()
    {
        // Toggle state only if allowed
        if (!_canAim) return;

        IsAiming = true;
        _canAim = false;
        _crosshair.DisableCrosshair();
        _animator.SetBool("Aim", true);

        UpdateFov();

        // Start coroutine to re-enable aiming after animation
        if (_aimCoroutine != null) StopCoroutine(_aimCoroutine);
        _aimCoroutine = StartCoroutine(WaitForAimingAnimation());
    }

    public virtual void StopAiming()
    {
        // Toggle state only if allowed
        if (!_canAim) return;

        IsAiming = false;
        _canAim = false;
        _crosshair.EnableCrosshair();
        _animator.SetBool("Aim", false);

        UpdateFov();

        // Start coroutine to re-enable aiming after animation
        if (_aimCoroutine != null) StopCoroutine(_aimCoroutine);
        _aimCoroutine = StartCoroutine(WaitForAimingAnimation());
    }

    // Re-enable aiming after aiming animation ends
    private IEnumerator WaitForAimingAnimation()
    {
        yield return new WaitForSeconds(_aimingAnimDuration);
        _canAim = true;
    }
}
