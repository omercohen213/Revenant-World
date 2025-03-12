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
    private Vector3 _movementDirection;
    private Vector3 _dashDirection;

    private PlayerInput _playerInput;
    private CharacterController _characterController;

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
        _currentCharges = _maxDashCharges; // Start with full 
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

        // Get movement direction (if not moving, dash forward)
        Vector3 moveDirection = new Vector3(_movementDirection.x, 0, _movementDirection.y).normalized;

        // If no movement input, dash forward (relative to player’s current facing direction)
        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.forward; // Dash forward if no movement input
        }
        else
        {
            // If there's movement input, use that to determine dash direction relative to the character's facing
            moveDirection = transform.TransformDirection(moveDirection); // Transform local direction to world direction
        }

        // Dash in the correct direction
        _dashDirection = moveDirection;

        float timer = 0f;
        while (timer < _dashDuration)
        {
            _characterController.Move(_dashDirection * _dashForce * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        _isDashing = false;

        // Start cooldown to regain charge
        StartCoroutine(RecoverDashCharge());
    }

    private IEnumerator RecoverDashCharge()
    {
        yield return new WaitForSeconds(_dashCooldown);
        if (_currentCharges < _maxDashCharges)
        {
            _currentCharges++;
        }
    }

}

