using GLTF.Schema;
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
    public Animator animator;
    [Tooltip("Position for weapons when active but not actively aiming")]
    public Transform DefaultWeaponPosition;
    [Tooltip("Crosshair")]
    public Crosshair crosshair;

    [Header("Misc")]
    public bool IsReloading = false;

    [Tooltip("Field of view when not aiming")]
    public float DefaultFov = 60f;

    [Tooltip("Portion of the regular FOV to apply to the weapon camera")]
    public float WeaponFovMultiplier = 0.5f; //change to scope
    [Tooltip("Aiming Animation Speed")]
    public float AimingAnimationSpeed = 10f;
    [Tooltip("Delay before switching weapon a second time, to avoid recieving multiple inputs from mouse wheel")]
    public float WeaponSwitchDelay = 1f;
    [Tooltip("Layer to set FPS weapon gameObjects to")]
    public LayerMask FpsWeaponLayer;

    public bool IsAiming { get; private set; }

    private Player _player;
    private PlayerInput _playerInput;
    private InventoryManager _inventoryManager;
    private RangedWeapon _activeWeapon;

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
        SetFov(DefaultFov);
    }

    void Update()
    {
        if (_player.ActiveWeapon is RangedWeapon rangedWeapon)
        {
            _activeWeapon = rangedWeapon;
        }

        // shoot handling
        if (_activeWeapon == null) return;

        // Prevent actions while reloading
        if (IsReloading)
        {
            IsAiming = false;
            return;
        }

        // Handle fire input
        if (_playerInput.GetFireInputDown())
        {
            _activeWeapon.StartShooting();
        }

        if (_playerInput.GetFireInputHeld())
        {
            _activeWeapon.ContinueShooting();
        }

        if (_playerInput.GetFireInputReleased())
        {
            _activeWeapon.StopShooting();
        }

        if (_activeWeapon.RangedWeaponData.HasAmmo)
        {
            // Handle reload input or if the weapon runs out of ammo
            if (_playerInput.reload && !IsReloading && _inventoryManager.GetTotalAmmo(_activeWeapon) > 0
                || _activeWeapon.CurrentAmmo <= 0 && _inventoryManager.GetTotalAmmo(_activeWeapon) > 0)
            {
                StartReload();
            }
        }

        IsAiming = _playerInput.aim;
    }


    // Update various animated features in LateUpdate because it needs to override the animated arm position
    void LateUpdate()
    {
        if (_activeWeapon is RangedWeapon)
        {
            UpdateWeaponAiming();
            //UpdateWeaponBob();
            //UpdateWeaponRecoil();
        }
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
        IsReloading = true;
        _reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }

    // Handles the reloading process over time
    private IEnumerator ReloadCoroutine()
    {
        if (_activeWeapon == null) yield break;

        _activeWeapon.StartReloadAnimation();

        float reloadTime = _activeWeapon.RangedWeaponData.ReloadTime;
        yield return new WaitForSeconds(reloadTime);

        // Check if the weapon still exists before applying the reload
        if (_activeWeapon != null)
        {
            int ammoNeeded = _activeWeapon.RangedWeaponData.ClipSize - _activeWeapon.CurrentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _inventoryManager.GetTotalAmmo(_activeWeapon));

            _inventoryManager.UseAmmo(ammoToReload);
            _activeWeapon.Reload(ammoToReload);
        }

        StopReload();
    }

    // Properly stops the reloading process
    private void StopReload()
    {
        IsReloading = false;
        _reloadCoroutine = null;
        _activeWeapon.StopReloadAnimation();
    }

    private void UpdateWeaponAiming()
    {
        Weapon activeWeapon = _player.ActiveWeapon;
        if (activeWeapon == null) return;

        if (IsAiming)
        {
            crosshair.DisableCrosshair();
            animator.SetBool("Aim", true);
            SetFov(Mathf.Lerp(Camera.Lens.FieldOfView,
         WeaponFovMultiplier * DefaultFov, AimingAnimationSpeed * Time.deltaTime));
        }
        else
        {
            crosshair.EnableCrosshair();
            animator.SetBool("Aim", false);
            SetFov(Mathf.Lerp(Camera.Lens.FieldOfView, DefaultFov, AimingAnimationSpeed * Time.deltaTime));
        }
    }
}