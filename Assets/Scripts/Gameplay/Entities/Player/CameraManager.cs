using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class CameraManager : MonoBehaviour
{
    public CinemachineCamera Camera;
    [SerializeField] private GameObject _cameraTarget;
    [SerializeField] private GameObject _rightArm;
    [SerializeField] private GameObject _leftArm;

    [Header("Camera Settings")]
    [SerializeField] private float _rotationSpeed = 1.0f;
    [SerializeField] private float _topClamp = 50f;
    [SerializeField] private float _bottomClamp = -60.0f;
    [SerializeField] private float _defaultFov = 60f;

    private PlayerInput _playerInput;
    private Coroutine _fovCoroutine;
    private Vector2 _currentLookDirection;
    private float _cinemachineTargetPitch;
    private float _rotationVelocity;

    private const float _CameraThreshold = 0.01f;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerInput.OnLookInput += HandleLookInput;
    }
    private void OnDisable()
    {
        _playerInput.OnLookInput -= HandleLookInput;
    }

    private void HandleLookInput(Vector2 lookInput)
    {
        _currentLookDirection = lookInput;
    }

    void Start()
    {
        SetFov(_defaultFov);

        // Parent arms and weapon to camera target to move them with the camera
         _rightArm.transform.SetParent(_cameraTarget.transform, true);
        _leftArm.transform.SetParent(_cameraTarget.transform, true);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        // if there is an input
        if (_currentLookDirection.sqrMagnitude >= _CameraThreshold)
        {
            // Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetPitch += _currentLookDirection.y * _rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _currentLookDirection.x * _rotationSpeed * deltaTimeMultiplier;

            // Clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

            // Apply rotation: Camera target controls pitch, player controls yaw
            _cameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void StartFovTransition(float targetFov, float duration)
    {
        if (_fovCoroutine != null)
        {
            StopCoroutine(_fovCoroutine);
        }
        _fovCoroutine = StartCoroutine(UpdateFovCoroutine(targetFov, duration));
    }

    private IEnumerator UpdateFovCoroutine(float targetFov, float duration)
    {
        float startFov = Camera.Lens.FieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Normalized progress 0-1
            SetFov(Mathf.Lerp(startFov, targetFov, t));
            yield return null;
        }

        SetFov(targetFov);
    }

    private void SetFov(float fov)
    {
        Camera.Lens.FieldOfView = fov;
    }
}
