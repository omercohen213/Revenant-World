using NaughtyAttributes;
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
    private CharacterController _characterController;
    private Vector3 _dashDirection;
    private PlayerInput _playerInput;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if(DebugUtil.SafeGetComponent(gameObject, out _playerInput)) return;

    }

    private void Start()
    {
        _currentCharges = _maxDashCharges; // Start with full 
    }

    void Update()
    {
        if (_playerInput == null) return;


        // Dash input check
        if (_playerInput.GetDashInputDown() && _currentCharges > 0 && !_isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        _isDashing = true;
        _currentCharges--;

        // Get movement direction (if not moving, dash forward)
        Vector3 moveDirection = new Vector3(_playerInput.Move.x, 0, _playerInput.Move.y).normalized;

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

    IEnumerator RecoverDashCharge()
    {
        yield return new WaitForSeconds(_dashCooldown);
        if (_currentCharges < _maxDashCharges)
        {
            _currentCharges++;
        }
    }

    /// <summary>
    /// Checks if the player can dash (useful for UI indicators).
    /// </summary>
    public bool CanDash()
    {
        return _currentCharges > 0 && !_isDashing;
    }
}

