using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(PlayerInput))]
public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Main Camera")]
    public CinemachineCamera Camera;
    [Tooltip("Animator")]
    private Animator _animator;
    [Tooltip("Position for weapons when active but not actively aiming")]
    [SerializeField] private Transform _defaultWeaponPosition;
    [Tooltip("Crosshair")]
    [SerializeField] private Crosshair _crosshair;

    [Header("Aiming")]
    private float _currentFov;
    [SerializeField] private bool IsReloading = false;
    [Tooltip("Field of view when not aiming")]
    [SerializeField] private float _defaultFov = 60f;
    private bool _previousAimingState = false;
    [Tooltip("Portion of the regular FOV to apply to the weapon camera")]
    public float WeaponFovMultiplier = 0.5f; //change to scope
    [Tooltip("Aiming Animation Speed")]
    public float AimingAnimationSpeed = 10f;


    [Header("Misc")]
    [Tooltip("Delay before switching weapon a second time, to avoid recieving multiple inputs from mouse wheel")]
    public float WeaponSwitchDelay = 1f;
    [Tooltip("Layer to set FPS weapon gameObjects to")]
    public LayerMask FpsWeaponLayer;

    public bool IsAiming { get; private set; }

    private Player _player;
    private PlayerInput _playerInput;
    private InventoryManager _inventoryManager;
    private Weapon _activeWeapon;
    private RangedWeapon _rangedWeapon;
    private MeleeWeapon _meleeWeapon;

    private float _weaponBobFactor;
    private Vector3 _lastCharacterPosition;
    private Vector3 _weaponMainLocalPosition;
    private Vector3 _weaponBobLocalPosition;
    private Vector3 _weaponRecoilLocalPosition;

    private Coroutine _reloadCoroutine;

    void Start()
    {
        if (!DebugUtil.SafeGetComponent(gameObject, out _playerInput)) return;
        if (!DebugUtil.SafeGetComponent(gameObject, out _inventoryManager)) return;
        if (!DebugUtil.SafeGetComponent(gameObject, out _player)) return;

        Camera = transform.Find("PlayerFollowCamera").GetComponent<CinemachineCamera>();
        _animator = GetComponentInChildren<Animator>(); 


        SetFov(_defaultFov);
        _currentFov = _defaultFov;
    }

    void Update()
    {
        UpdateActiveWeapon();
        if (_activeWeapon == null) return;
        HandleWeaponActions();
    }

    void UpdateActiveWeapon()
    {
        if (_player.ActiveWeapon == _activeWeapon) return; // Only update if the weapon changes

        _activeWeapon = _player.ActiveWeapon;

        _rangedWeapon = _activeWeapon as RangedWeapon;
        _meleeWeapon = _activeWeapon as MeleeWeapon;
    }

    void HandleWeaponActions()
    {
        if (_rangedWeapon != null)
        {
            HandleRangedWeaponActions();
        }
        else if (_meleeWeapon != null)
        {
            HandleMeleeWeaponActions();
        }
    }

    void HandleRangedWeaponActions()
    {
        // Prevent actions while reloading
        if (IsReloading)
        {
            IsAiming = false;
            return;
        }

        // Handle aim
        IsAiming = _playerInput.aim;
        UpdateWeaponAiming();
        UpdateFov();


        // Handle fire input
        if (_playerInput.GetFireInputDown())
        {
            _rangedWeapon.StartShooting();
        }

        if (_playerInput.GetFireInputHeld())
        {
            _rangedWeapon.ContinueShooting();
        }

        if (_playerInput.GetFireInputReleased())
        {
            _rangedWeapon.StopShooting();
        }

        if (_rangedWeapon.RangedWeaponData.HasAmmo)
        {
            int totalAmmo = _inventoryManager.GetTotalAmmo(_rangedWeapon);
            int ammoNeeded = _rangedWeapon.RangedWeaponData.ClipSize - _rangedWeapon.CurrentAmmo;

            if (_playerInput.reload && !IsReloading && totalAmmo > 0 && ammoNeeded > 0 ||
                _rangedWeapon.CurrentAmmo <= 0 && totalAmmo > 0)
            {
                StartReload();
            }
        }
    }

    void HandleMeleeWeaponActions()
    {
        // Handle melee attacks, if necessary
    }

    // Sets the FOV of the main camera
    public void SetFov(float fov)
    {
        Camera.Lens.FieldOfView = fov;
    }

    // Handle the start of a reload
    private void StartReload()
    {
        if (_reloadCoroutine != null) return; // Prevent multiple reloads at once

        // Stop aiming
        if (IsAiming)
        {
            IsAiming = false;
            UpdateWeaponAiming();
            SetFov(_defaultFov);
        }

        IsReloading = true;
        _reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }

    // Handles the reloading process over time. Is only for ranged weapons
    private IEnumerator ReloadCoroutine()
    {
        if (_rangedWeapon == null) yield break;

        _rangedWeapon.StartReloadAnimation();

        float reloadTime = _rangedWeapon.RangedWeaponData.ReloadTime;
        yield return new WaitForSeconds(reloadTime);

        // Check if the weapon still exists before applying the reload
        if (_activeWeapon != null)
        {
            int ammoNeeded = _rangedWeapon.RangedWeaponData.ClipSize - _rangedWeapon.CurrentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _inventoryManager.GetTotalAmmo(_rangedWeapon));

            _inventoryManager.UseAmmo(ammoToReload);
            _rangedWeapon.Reload(ammoToReload);
        }

        StopReload();
    }

    // Properly stops the reloading process
    private void StopReload()
    {
        IsReloading = false;
        _reloadCoroutine = null;
        _rangedWeapon.StopReloadAnimation();
    }
    private void UpdateFov()
    {
        float targetFov = IsAiming ? WeaponFovMultiplier * _defaultFov : _defaultFov;
        _currentFov = Mathf.Lerp(_currentFov, targetFov, AimingAnimationSpeed * Time.deltaTime);
        SetFov(_currentFov);
    }

    private void UpdateWeaponAiming()
    {
        Weapon activeWeapon = _player.ActiveWeapon;
        if (activeWeapon == null) return;
        if (IsAiming == _previousAimingState) return;
        _previousAimingState = IsAiming;

        if (IsAiming)
        {
            _crosshair.DisableCrosshair();
            _animator.SetBool("Aim", true);
        }
        else
        {
            _crosshair.EnableCrosshair();
            _animator.SetBool("Aim", false);
        }
    }
}