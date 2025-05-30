using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    [Tooltip("Force applied when dashing")]
    [SerializeField] private float _dashForce = 15f;

    [Tooltip("Duration of the dash movement")]
    [SerializeField] private float _dashDuration = 0.2f;

    [Tooltip("Cooldown time per dash charge")]
    [SerializeField] private float _dashCooldown = 5f;

    [Tooltip("Maximum number of dash charges")]
    [SerializeField] private int _maxDashCharges = 2;

    [ProgressBar("_currentCharges", "_maxDashCharges", EColor.Gray)]
    [SerializeField] private int _currentCharges;

    private bool _isDashing;
    private Vector3 _dashVelocity;
    private Vector3 _movementDirection;
    private Vector3 _dashDirection;

    private PlayerInput _playerInput;
    private CharacterController _characterController;

    public event Action<float, int> OnDashRecovery; // Invoke the event with cooldown and currentDashes

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerInput.OnDashPressed += HandleDashPressed;
        _playerInput.OnMoveInput += HandleMoveInput;
    }

    private void OnDisable()
    {
        _playerInput.OnDashPressed -= HandleDashPressed;
        _playerInput.OnMoveInput -= HandleMoveInput;
    }

    private void Start()
    {
        _currentCharges = 0; // Start with no charges
        StartCoroutine(RecoverAllDashCharges());
    }

    private void HandleDashPressed()
    {
        if (CanDash())
        {
            StartCoroutine(PerformDash());
        }
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _movementDirection = moveInput;
    }

    public bool CanDash()
    {
        return _currentCharges > 0 && !_isDashing;
    }

    private IEnumerator PerformDash()
    {
        _isDashing = true;
        _currentCharges--;

        // Determine dash direction
        Vector3 moveDirection = new Vector3(_movementDirection.x, 0, _movementDirection.y).normalized;
        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.forward; // Dash forward if no movement input
        }
        else
        {
            moveDirection = transform.TransformDirection(moveDirection); // Convert local to world direction
        }

        // Apply dash force
        _dashVelocity = moveDirection * _dashForce;

        // Wait for dash duration
        yield return new WaitForSeconds(_dashDuration);

        _dashVelocity = Vector3.zero; // Reset dash movement
        _isDashing = false;

        // Recover charge
        OnDashRecovery?.Invoke(_dashCooldown, _currentCharges);
        StartCoroutine(RecoverDashCharge());
    }

    private void FixedUpdate()
    {
        // Apply dash velocity in FixedUpdate
        if (_isDashing)
        {
            _characterController.Move(_dashVelocity * Time.fixedDeltaTime);
        }
    }

    private IEnumerator RecoverDashCharge()
    {

        yield return new WaitForSeconds(_dashCooldown);
        if (_currentCharges < _maxDashCharges)
        {
            _currentCharges++;
        }
    }
    private IEnumerator RecoverAllDashCharges()
    {
        while (_currentCharges < _maxDashCharges)
        {
            OnDashRecovery?.Invoke(_dashCooldown, _currentCharges);

            // Wait for cooldown before adding one charge
            yield return new WaitForSeconds(_dashCooldown);

            // Only recover a charge if it's not already full
            if (_currentCharges < _maxDashCharges)
            {
                _currentCharges++;
            }
        }
    }
}

