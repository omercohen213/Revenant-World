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
    public event Action<WeaponSwitchCommand> OnWeaponSwitched;

    public event Action OnInventoryPressed;
    public event Action OnSettingsPressed;

    private Vector2 _move;
    private Vector2 _look;
    private bool _sprintHeld;
    private bool _fireDown;
    private bool _fireHeld;
    private bool _fireReleased;

    private bool _isUIOpen = false;
    /*
        private UICursor _uiCursor;

        private void Awake()
        {
            _uiCursor = GetComponentInChildren<UICursor>();
        }*/

    private void Update()
    {
        InvokeMoveInput();
        CheckSprintHeld();

        // Block input when UI is open
        if (_isUIOpen)
        {
            // When UI is open, reset look input to zero so camera stops rotating.
            _look = Vector2.zero;
            InvokeLookInput();
        }
        else
        {
            InvokeLookInput();
            CheckFireHeld();
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

    // Continous actions
    public void OnMove(InputValue value) => _move = value.Get<Vector2>();
    public void OnLook(InputValue value)
    {
        if (_isUIOpen)
            _look = Vector2.zero;
        else
            _look = value.Get<Vector2>();
    }
    public void OnSprint(InputValue value) => _sprintHeld = value.isPressed;
    public void OnFire(InputValue value)
    {
        if (_isUIOpen) return; // Block input when UI is open

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
    public void OnAim(InputValue value)
    {
        if (_isUIOpen) return; // Block input when UI is open
        if (value.isPressed)
        {
            OnAimPressed?.Invoke();
        }
    }
    public void OnPickUp(InputValue value) { if (value.isPressed) OnPickUpPressed?.Invoke(); }
    public void OnInventory(InputValue value)
    {
        if (value.isPressed)
        {
            _isUIOpen = !_isUIOpen;
            OnInventoryPressed?.Invoke();
        }
    }
    public void OnSettings(InputValue value)
    {
        if (value.isPressed)
        {
            _isUIOpen = !_isUIOpen;
            OnSettingsPressed?.Invoke();
        }
    }
    public void OnSwitchToItem1(InputValue value) { if (value.isPressed) OnWeaponSwitched?.Invoke(
        new WeaponSwitchCommand(WeaponSwitchType.DirectSelect, 1)); }
    public void OnSwitchToItem2(InputValue value)
    {
        if (value.isPressed) OnWeaponSwitched?.Invoke(
        new WeaponSwitchCommand(WeaponSwitchType.DirectSelect, 2));
    }
    public void OnSwitchToItem3(InputValue value)
    {
        if (value.isPressed) OnWeaponSwitched?.Invoke(
        new WeaponSwitchCommand(WeaponSwitchType.DirectSelect, 3));
    }
    public void OnSwitchToItem4(InputValue value)
    {
        if (value.isPressed) OnWeaponSwitched?.Invoke(
        new WeaponSwitchCommand(WeaponSwitchType.DirectSelect, 4));
    }
    public void OnSwitchToItem5(InputValue value)
    {
        if (value.isPressed) OnWeaponSwitched?.Invoke(
        new WeaponSwitchCommand(WeaponSwitchType.DirectSelect, 5));
    }
    public void OnSwitchToItem6(InputValue value)
    {
        if (value.isPressed) OnWeaponSwitched?.Invoke(
        new WeaponSwitchCommand(WeaponSwitchType.DirectSelect, 6));
    }
    public void OnScrollWheel(InputValue value)
    {
        float scrollDelta = value.Get<float>();

        if (scrollDelta > 0)
        {
            OnWeaponSwitched?.Invoke(new WeaponSwitchCommand(WeaponSwitchType.ScrollUp));
        }
        else if (scrollDelta < 0)
        {
            OnWeaponSwitched?.Invoke(new WeaponSwitchCommand(WeaponSwitchType.ScrollDown));
        }
    }
}

public enum WeaponSwitchType
{
    ScrollUp,
    ScrollDown,
    DirectSelect
}

public struct WeaponSwitchCommand
{
    public WeaponSwitchType Type;
    public int Index; // Only used when Type == DirectSelect

    public WeaponSwitchCommand(WeaponSwitchType type, int index = -1)
    {
        Type = type;
        Index = index;
    }
}