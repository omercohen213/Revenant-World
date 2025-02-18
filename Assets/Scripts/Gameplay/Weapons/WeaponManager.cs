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
        SetFov(DefaultFov);
    }

    void Update()
    {
        if (_inventoryManager.GetActiveWeapon() is RangedWeapon rangedWeapon)
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

    // Updates weapon position and camera FoV for the aiming transition
    /*private void UpdateWeaponAiming()
    {
        Weapon activeWeapon = _inventoryManager.GetActiveWeapon();
        if (activeWeapon == null) return;

        float speed = AimingAnimationSpeed * Time.deltaTime;

        if (_playerInput.aim)
        {
            IsAiming = true;

            // Move Aim Camera to the Scope's Position for a perfect alignment
            AimCamera.transform.position = Vector3.Lerp(AimCamera.transform.position,
                activeWeapon.ScopeTransform.position, speed * 5f); // Faster movement

            // Look at the aiming direction
            AimCamera.transform.rotation = Quaternion.Lerp(AimCamera.transform.rotation,
                activeWeapon.ScopeTransform.rotation, speed * 5f);

            // Increase AimCam priority
            AimCamera.Priority = 11;
            DefaultCamera.Priority = 9;

            // Move weapon to aiming position
            _weaponMainLocalPosition = Vector3.Lerp(_weaponMainLocalPosition,
                AimingWeaponPosition.localPosition + activeWeapon.WeaponData.AimOffset, speed);

            // Adjust FOV smoothly
            SetFov(Mathf.Lerp(_playerController.Camera.Lens.FieldOfView,
                activeWeapon.WeaponData.AimZoomRatio * DefaultFov, speed));
        }
        else
        {
            IsAiming = false;

            // Reset Aim Camera Position
            AimCamera.Priority = 9;
            DefaultCamera.Priority = 11;

            // Reset weapon position
            _weaponMainLocalPosition = Vector3.Lerp(_weaponMainLocalPosition,
                DefaultWeaponPosition.localPosition, speed);

            // Reset FOV smoothly
            SetFov(Mathf.Lerp(_playerController.Camera.Lens.FieldOfView, DefaultFov, speed));
        }
    }*/

    private void UpdateWeaponAiming()
    {
        Weapon activeWeapon = _inventoryManager.GetActiveWeapon();
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



    /*// Updates the weapon bob animation based on character speed
    void UpdateWeaponBob()
    {
        Weapon activeWeapon = _inventoryManager.GetActiveWeapon();

        if (activeWeapon == null) return;

        // Calculate bobbing effect
        float bobAmount = IsAiming ? activeWeapon.WeaponData.AimingBobAmount : activeWeapon.WeaponData.DefaultBobAmount;
        float frequency = activeWeapon.WeaponData.BobFrequency;
        float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * _weaponBobFactor;
        float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * _weaponBobFactor;

        // Apply other influences like aiming or recoil
        Vector3 aimAdjustment = GetAimingAdjustment(); // For example, offset based on aiming
        Vector3 recoilAdjustment = GetRecoilAdjustment(); // Recoil-based offset

        // Combine all influences
        Vector3 finalWeaponPosition = DefaultWeaponPosition.transform.position
                                      + new Vector3(hBobValue, Mathf.Abs(vBobValue), 0f)
                                      + aimAdjustment
                                      + recoilAdjustment;

        // Apply the final position to the weapon socket
        WeaponSocket.position = finalWeaponPosition;
    }*/


    Vector3 GetRecoilAdjustment()
    {
        // Simulate recoil, this could be based on weapon type, or firing
        return new Vector3(0f, 0.1f, -0.05f); // Example recoil effect
    }

    /*void UpdateWeaponRecoil()
    {
        Weapon activeWeapon = _inventoryManager.GetActiveWeapon();
        if (activeWeapon == null) return;

        // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
        if (_weaponRecoilLocalPosition.z >= _accumulatedRecoil.z * 0.99f)
        {
            _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, _accumulatedRecoil,
                activeWeapon.WeaponData.RecoilSharpness * Time.deltaTime);
        }
        // otherwise, move recoil position to make it recover towards its resting pose
        else
        {
            _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, Vector3.zero,
                activeWeapon.WeaponData.RecoilRestitutionSharpness * Time.deltaTime);
            _accumulatedRecoil = _weaponRecoilLocalPosition;
        }
    }*/

}