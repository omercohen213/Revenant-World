using System;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : EntityMovement
{
    private PlayerInput _playerInput;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float _moveSpeed = 4.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float _sprintSpeed = 6.0f;
    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float _speedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField] private float _jumpHeight = 1f;
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float _jumpTimeout = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float _fallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] private bool _isGrounded = true;
    [Tooltip("Useful for rough ground")]
    [SerializeField] private float _groundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float _groundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask _groundLayers;

    private bool _jumpRequested = false;
    private bool _isSprinting;
    private float _currentSpeed;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private readonly float _maxVelocity = 53.0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerInput.OnMoveInput += HandleMoveInput;
        _playerInput.OnJumpPressed += HandleJumpPressed;
        _playerInput.OnSprintHeld += HandleSprintHeld;
    }

    private void OnDisable()
    {
        _playerInput.OnMoveInput -= HandleMoveInput;
        _playerInput.OnJumpPressed -= HandleJumpPressed;
        _playerInput.OnSprintHeld -= HandleSprintHeld;

    }

    private void Start()
    {
        // reset timeouts on start
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
        GroundedCheck();
    }

    private void Update()
    {
        ApplyGravity();
        ProcessJump();
        GroundedCheck();
        Move();

        // Reset jump request after processing
        _jumpRequested = false;
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _currentMovement = moveInput;
    }

    // Only set jump request; process it in Update
    private void HandleJumpPressed()
    {
        _jumpRequested = true;
    }
    private void HandleSprintHeld(bool sprintValue)
    {
        // Prevent sprinting if moving backward
        if (_currentMovement.y < 0)
        {
            _isSprinting = false;
        }
        else
        {
            _isSprinting = sprintValue;
        }
    }
    private void ProcessJump()
    {
        if (_isGrounded)
        {
            _fallTimeoutDelta = _fallTimeout;

            if (_jumpRequested && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            // Count down jump timeout
            if (_jumpTimeoutDelta > 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = _jumpTimeout;

            if (_fallTimeoutDelta > 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {
        // Calculate the movement direction using the character's transform
        Vector3 inputDirection = (transform.right * _currentMovement.x + transform.forward * _currentMovement.y).normalized;

        // Prevent sprinting if moving backward
        if (_currentMovement.y < 0)
        {
            _isSprinting = false;
        }

        // Determine target speed based on whether sprinting
        float targetSpeed = _isSprinting ? _sprintSpeed : _moveSpeed;
        if (_currentMovement == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        // Calculate current horizontal speed (ignoring vertical velocity)
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        // Smoothly adjust the current speed towards the target speed.
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);
            _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
        }
        else
        {
            _currentSpeed = targetSpeed;
        }

        base.Move(inputDirection, _currentSpeed);
    }

}
