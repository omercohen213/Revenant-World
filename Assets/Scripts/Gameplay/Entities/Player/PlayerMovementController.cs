using System;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovementController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _controller;

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
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float _gravity = -9.81f;

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

    private Vector2 _currentMovement;
    private bool _isSprinting;
    private float _currentSpeed;
    private float _verticalVelocity;
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
        _isGrounded = true;
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }

    private void Update()
    {
        JumpAndGravity(false);
        GroundedCheck();
        Move();
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        _currentMovement = moveInput;
    }
    private void HandleJumpPressed()
    {
        JumpAndGravity(true);
    }
    private void HandleSprintHeld(bool sprintValue)
    {
        _isSprinting = sprintValue;
    }
    private void JumpAndGravity(bool jumpPressed)
    {
        if (_isGrounded)
        {
            // Reset the fall timeout timer
            _fallTimeoutDelta = _fallTimeout;

            // Stop velocity from going infinitely down when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump (only if triggered by event)
            if (jumpPressed && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            // Jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // Reset the jump timeout timer
            _jumpTimeoutDelta = _jumpTimeout;

            // Fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
        }

        // Apply gravity over time if under terminal velocity
        if (_verticalVelocity < _maxVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
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
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _isSprinting ? _sprintSpeed : _moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no movement, set the target speed to 0
        if (_currentMovement == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed , Time.deltaTime * _speedChangeRate);

            // round speed to 3 decimal places
            _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
        }
        else
        {
            _currentSpeed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_currentMovement.x, 0.0f, _currentMovement.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_currentMovement != Vector2.zero)
        {
            // move
            inputDirection = transform.right * _currentMovement.x + transform.forward * _currentMovement.y;
        }

        // move the player
        _controller.Move(inputDirection.normalized * (_currentSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

}
