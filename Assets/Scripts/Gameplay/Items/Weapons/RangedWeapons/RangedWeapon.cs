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
    protected float _weaponFovMultiplier = 0.5f; //change to scope

    private bool _isAiming = false;
    private bool _canAim = true;
    private readonly float _aimingAnimDuration = 0.3f;
    private Coroutine _reloadCoroutine;
    private Coroutine _aimCoroutine;

    protected void InvokeShoot() => OnShoot?.Invoke();
    protected void InvokeReload() => OnReload?.Invoke();

    protected virtual void OnEnable()
    {
        _playerInput.OnReloadPressed += HandleReloadPressed;
        _playerInput.OnAimPressed += HandleAimPressed;
        _playerInput.OnFireDown += HandleFireDown;
        _playerInput.OnFireHeld += HandleFireHeld;
        _playerInput.OnFireReleased += HandleFireReleased;
    }

    protected virtual void OnDisable()
    {
        _playerInput.OnReloadPressed -= HandleReloadPressed;
        _playerInput.OnAimPressed -= HandleAimPressed;
        _playerInput.OnFireDown -= HandleFireDown;
        _playerInput.OnFireHeld -= HandleFireHeld;
        _playerInput.OnFireReleased -= HandleFireReleased;

    }

    protected override void Update()
    {
        CheckReloading();
        CheckAutomaticReload();
    }

    // Prevent actions while reloading
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
    }

    // Reload automatically if no ammo left
    private void CheckAutomaticReload()
    {
        if (CurrentAmmo <= 0)
        {
            StartReloading();
            return;
        }
    }

    private void HandleReloadPressed()
    {
        if (CanReload())
        {
            StartReloading();
        }
    }

    private void HandleAimPressed()
    {
        if (_canAim && !_isReloading)
        {
            if (!_isAiming)
            {
                StartAiming();
            }
            else
            {
                StopAiming();
            }
        }
    }

    public virtual void HandleFireDown()
    {
    }

    public virtual void HandleFireHeld()
    {
    }

    public virtual void HandleFireReleased()
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
            return !_isReloading && totalAmmo > 0 && ammoNeeded > 0 || CurrentAmmo <= 0 && totalAmmo > 0;
        }
        else return false;
    }

    // Handle the start of a reload
    private void StartReloading()
    {
        if (_reloadCoroutine != null) return; // Prevent multiple reloads at once

        // Stop aiming
        if (_isAiming)
        {
            StopAiming();
            _isAiming = false;
        }

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

    private void UpdateFov()
    {
        float targetFov = _isAiming ? _weaponFovMultiplier * _defaultFov : _defaultFov;
        _cameraManager.StartFovTransition(targetFov, _aimingAnimDuration);
    }

    public virtual void StartAiming()
    {
        // Toggle state only if allowed
        if (!_canAim) return;

        _isAiming = true;
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

        _isAiming = false;
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
