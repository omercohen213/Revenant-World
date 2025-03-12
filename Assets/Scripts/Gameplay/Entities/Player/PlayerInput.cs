using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2> OnLookInput;
    public event Action<bool> OnSprintHeld;
    public event Action OnFireDown;
    public event Action OnFireHeld;
    public event Action OnFireReleased;

    public event Action OnJumpPressed;
    public event Action OnDashPressed;
    public event Action OnCrouchPressed;
    public event Action OnReloadPressed;
    public event Action OnAimPressed;
    public event Action OnPickUpPressed;

    private Vector2 _move;
    private Vector2 _look;
    private bool _sprintHeld;
    private bool _fireDown;
    private bool _fireHeld;
    private bool _fireReleased;

    private void Update()
    {
        LockCursor();
        InvokeMoveInput();
        InvokeLookInput();
        CheckFireHeld();
        CheckSprintHeld();
    }

    private void LockCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void InvokeMoveInput()
    {
        OnMoveInput?.Invoke(_move);
    }
    private void InvokeLookInput()
    {
        OnLookInput?.Invoke(_look);
    }

    private void CheckSprintHeld()
    {
        if (_sprintHeld)
        {
            OnSprintHeld?.Invoke(true);
        }
        else
        {
            OnSprintHeld?.Invoke(false);
        }
    }
    private void CheckFireHeld()
    {
        if (_fireHeld)
        {
            OnFireHeld?.Invoke();
        }
    }

#if ENABLE_INPUT_SYSTEM
    // Continous actions
    public void OnMove(InputValue value) => _move = value.Get<Vector2>();
    public void OnLook(InputValue value) => _look = value.Get<Vector2>();
    public void OnSprint(InputValue value) => _sprintHeld = value.isPressed;
    public void OnFire(InputValue value)
    {
        _fireDown = !_fireHeld && value.isPressed;
        _fireHeld = value.isPressed;
        _fireReleased = _fireHeld && !value.isPressed;

        // Trigger events based on input state
        if (_fireDown) OnFireDown?.Invoke();
        if (_fireHeld) _fireHeld = true;
        if (_fireReleased)
        {
            OnFireReleased?.Invoke();
            _fireHeld = false;
        }
    }

    // One-Frame actions
    public void OnJump(InputValue value) { if (value.isPressed) OnJumpPressed?.Invoke(); }
    public void OnDash(InputValue value) { if (value.isPressed) OnDashPressed?.Invoke(); }
    public void OnCrouch(InputValue value) { if (value.isPressed) OnCrouchPressed?.Invoke(); }
    public void OnReload(InputValue value) { if (value.isPressed) OnReloadPressed?.Invoke(); }
    public void OnAim(InputValue value) { if (value.isPressed) OnAimPressed?.Invoke(); }
    public void OnPickUp(InputValue value) { if (value.isPressed) OnPickUpPressed?.Invoke(); }

#endif
}
