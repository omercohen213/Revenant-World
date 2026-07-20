using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CameraManager))]

public class WeaponAimController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CameraManager _cameraManager;
    private WeaponController _weaponManager;
    private Animator _animator;

    private float _weaponFovMultiplier = 0.5f;

    private bool _isAiming = false;
    private bool _canAim = true;
    private readonly float _aimingAnimDuration = 0.3f;
    private Coroutine _reloadCoroutine;
    private Coroutine _aimCoroutine;

    private void OnEnable()
    {
        _playerInput.OnAimPressed += HandleAimPressed;

    }

    private void OnDisable()
    {
        _playerInput.OnAimPressed -= HandleAimPressed;

    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _cameraManager = GetComponent<CameraManager>();
        _weaponManager = GetComponent<WeaponController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {

    }

    private void HandleAimPressed()
    {
        if (_weaponManager.ActiveWeapon is not RangedWeapon currentWeapon)
            return;

        if (_canAim && !currentWeapon.IsReloading)
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

    public virtual void StartAiming()
    {
        // Toggle state only if allowed
        if (!_canAim) return;

        _isAiming = true;
        _canAim = false;
        //_crosshair.DisableCrosshair();
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

        if (_weaponManager.ActiveWeapon is not RangedWeapon currentWeapon)
            return;
        currentWeapon.Crosshair.EnableCrosshair();
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

    private void UpdateFov()
    {
        float targetFov = _isAiming ? _weaponFovMultiplier * _cameraManager.DefaultFov : _cameraManager.DefaultFov;
        _cameraManager.StartFovTransition(targetFov, _aimingAnimDuration);
    }
}
