using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInput))]
public class PlayerWeaponManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Default Camera")]
    public CinemachineCamera DefaultCamera;

    [Tooltip("Seperate Camera for aiming")]
    public CinemachineCamera AimCamera;

    //[Tooltip("Parent transform where all weapon will be added in the hierarchy")]
    //public Transform WeaponParentSocket;

    [Tooltip("Position for weapons when active but not actively aiming")]
    public Transform DefaultWeaponPosition;

    [Tooltip("Position for weapons when aiming")]
    public Transform AimingWeaponPosition;

    [Tooltip("Position for innactive weapons")]
    public Transform DownWeaponPosition;

    [Header("Weapon Bob")]
    [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
    public float BobFrequency = 10f;

    [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
    public float BobSharpness = 10f;

    [Tooltip("Distance the weapon bobs when not aiming")]
    public float DefaultBobAmount = 0.05f;

    [Tooltip("Distance the weapon bobs when aiming")]
    public float AimingBobAmount = 0.02f;

    [Header("Weapon Recoil")]
    [Tooltip("This will affect how fast the recoil moves the weapon, the bigger the value, the fastest")]
    public float RecoilSharpness = 50f;

    [Tooltip("Maximum distance the recoil can affect the weapon")]
    public float MaxRecoilDistance = 0.5f;

    [Tooltip("How fast the weapon goes back to it's original position after the recoil is finished")]
    public float RecoilRestitutionSharpness = 10f;

    [Header("Misc")]
    [Tooltip("Speed at which the aiming animatoin is played")]
    public float AimingAnimationSpeed = 10f;

    [Tooltip("Field of view when not aiming")]
    public float DefaultFov = 60f;

    [Tooltip("Portion of the regular FOV to apply to the weapon camera")]
    public float WeaponFovMultiplier = 0.5f;

    [Tooltip("Delay before switching weapon a second time, to avoid recieving multiple inputs from mouse wheel")]
    public float WeaponSwitchDelay = 1f;

    [Tooltip("Layer to set FPS weapon gameObjects to")]
    public LayerMask FpsWeaponLayer;

    public bool IsAiming { get; private set; }
    public bool IsPointingAtEnemy { get; private set; }
    public int ActiveWeaponIndex { get; private set; }

    private PlayerInput _playerInput;
    private PlayerController _PlayerController;
    private InventoryManager _inventoryManager;
    private float m_WeaponBobFactor;
    private Vector3 m_LastCharacterPosition;
    private Vector3 m_WeaponMainLocalPosition;
    private Vector3 m_WeaponBobLocalPosition;
    private Vector3 m_WeaponRecoilLocalPosition;
    private Vector3 m_AccumulatedRecoil;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerInput, PlayerWeaponManager>(_playerInput, this,
            gameObject);

        _PlayerController = GetComponent<PlayerController>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerWeaponsManager>(
            _PlayerController, this, gameObject);

        _inventoryManager = GetComponent<InventoryManager>();
        DebugUtility.HandleErrorIfNullGetComponent<InventoryManager, PlayerWeaponsManager>(
            _inventoryManager, this, gameObject);

        SetFov(DefaultFov);
    }

    void Update()
    {
        // shoot handling
        Weapon activeWeapon = _inventoryManager.GetActiveWeapon();

        if (activeWeapon != null && activeWeapon.IsReloading)
            return;

        if (activeWeapon != null)
        {
            if (!activeWeapon.AutomaticReload && _playerInput.reload && activeWeapon.CurrentAmmoRatio < 1.0f)
            {
                IsAiming = false;
                activeWeapon.StartReloadAnimation();
                return;
            }

            // handle aiming down sights
            IsAiming = _playerInput.aim;

            // handle shooting
            bool hasFired = activeWeapon.HandleShootInputs(
                _playerInput.GetFireInputDown(),
                _playerInput.GetFireInputHeld(),
                _playerInput.GetFireInputReleased());

            // Handle accumulating recoil
            if (hasFired)
            {
                m_AccumulatedRecoil += Vector3.back * activeWeapon.RecoilForce;
                m_AccumulatedRecoil = Vector3.ClampMagnitude(m_AccumulatedRecoil, MaxRecoilDistance);
            }
        }

        // Pointing at enemy handling
        IsPointingAtEnemy = false;
        if (activeWeapon)
        {
            if (Physics.Raycast(_PlayerController.Camera.transform.position, _PlayerController.Camera.transform.forward, out RaycastHit hit,
                1000, -1, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.GetComponentInParent<Health>() != null)
                {
                    IsPointingAtEnemy = true;
                }
            }
        }
    }


    // Update various animated features in LateUpdate because it needs to override the animated arm position
    void LateUpdate()
    {
        UpdateWeaponAiming();
        UpdateWeaponBob();
        UpdateWeaponRecoil();

       /* // Set final weapon socket position based on all the combined animation influences
        WeaponParentSocket.localPosition =
            m_WeaponMainLocalPosition + m_WeaponBobLocalPosition + m_WeaponRecoilLocalPosition;*/
    }

    // Sets the FOV of the main camera and the weapon camera simultaneously
    public void SetFov(float fov)
    {
        _PlayerController.Camera.Lens.FieldOfView = fov;
        AimCamera.Lens.FieldOfView = fov * WeaponFovMultiplier;
    }

    // Updates weapon position and camera FoV for the aiming transition
    void UpdateWeaponAiming()
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
            m_WeaponMainLocalPosition = Vector3.Lerp(m_WeaponMainLocalPosition,
                AimingWeaponPosition.localPosition + activeWeapon.AimOffset, speed);

            // Adjust FOV smoothly
            SetFov(Mathf.Lerp(_PlayerController.Camera.Lens.FieldOfView,
                activeWeapon.AimZoomRatio * DefaultFov, speed));
        }
        else
        {
            IsAiming = false;

            // Reset Aim Camera Position
            AimCamera.Priority = 9;
            DefaultCamera.Priority = 11;

            // Reset weapon position
            m_WeaponMainLocalPosition = Vector3.Lerp(m_WeaponMainLocalPosition,
                DefaultWeaponPosition.localPosition, speed);

            // Reset FOV smoothly
            SetFov(Mathf.Lerp(_PlayerController.Camera.Lens.FieldOfView, DefaultFov, speed));
        }
    }


    // Updates the weapon bob animation based on character speed
    void UpdateWeaponBob()
    {
        if (Time.deltaTime > 0f)
        {
            Vector3 playerCharacterVelocity =
                (_PlayerController.transform.position - m_LastCharacterPosition) / Time.deltaTime;

            // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
            float characterMovementFactor = 0f;
            if (_PlayerController.IsGrounded)
            {
                characterMovementFactor =
                    Mathf.Clamp01(playerCharacterVelocity.magnitude /_PlayerController.MoveSpeed);
            }

            m_WeaponBobFactor =
                Mathf.Lerp(m_WeaponBobFactor, characterMovementFactor, BobSharpness * Time.deltaTime);

            // Calculate vertical and horizontal weapon bob values based on a sine function
            float bobAmount = IsAiming ? AimingBobAmount : DefaultBobAmount;
            float frequency = BobFrequency;
            float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * m_WeaponBobFactor;
            float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                              m_WeaponBobFactor;

            // Apply weapon bob
            m_WeaponBobLocalPosition.x = hBobValue;
            m_WeaponBobLocalPosition.y = Mathf.Abs(vBobValue);

            m_LastCharacterPosition = _PlayerController.transform.position;
        }
    }

    // Updates the weapon recoil animation
    void UpdateWeaponRecoil()
    {
        // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
        if (m_WeaponRecoilLocalPosition.z >= m_AccumulatedRecoil.z * 0.99f)
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, m_AccumulatedRecoil,
                RecoilSharpness * Time.deltaTime);
        }
        // otherwise, move recoil position to make it recover towards its resting pose
        else
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, Vector3.zero,
                RecoilRestitutionSharpness * Time.deltaTime);
            m_AccumulatedRecoil = m_WeaponRecoilLocalPosition;
        }
    }
}

