using System;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerRuntimeData))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _controller;
    private PlayerRuntimeData _data;

    protected Vector2 _currentMovement;
    protected float _verticalVelocity; // for gravity
    private bool _jumpRequested = false;
    private bool _isSprinting;
    private bool _isGrounded;
    private float _currentSpeed;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private PlayerBaseData _baseData => _data.BaseData;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _data = GetComponent<PlayerRuntimeData>();
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
        _jumpTimeoutDelta = _baseData.JumpTimeout;
        _fallTimeoutDelta = _baseData.FallTimeout;
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
            _fallTimeoutDelta = _baseData.FallTimeout;

            if (_jumpRequested && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_baseData.JumpHeight * -2f * _baseData.Gravity);
            }

            // Count down jump timeout
            if (_jumpTimeoutDelta > 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = _baseData.JumpTimeout;

            if (_fallTimeoutDelta > 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new(transform.position.x, transform.position.y - _baseData.GroundedOffset, transform.position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, _baseData.GroundedRadius, _baseData.GroundLayers, QueryTriggerInteraction.Ignore);
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
        float targetSpeed = _isSprinting ? _data.MovementSpeed * _data.BaseData.SprintSpeedMultiplier : _data.MovementSpeed;
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
            _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _baseData.SpeedChangeRate);
            _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
        }
        else
        {
            _currentSpeed = targetSpeed;
        }

        Vector3 movement = inputDirection * _currentSpeed;
        movement.y = _verticalVelocity;
        _controller.Move(movement * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }

        _verticalVelocity += _baseData.Gravity * Time.deltaTime;
    }

}
