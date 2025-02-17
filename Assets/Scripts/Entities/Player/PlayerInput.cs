using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    #region Character Input Values
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool dash;

    public bool fireDown;
    public bool fireHeld;
    public bool fireReleased;
    public bool aim;
    public bool crouch;
    public bool reload;
    #endregion

    #region Settings
    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
    #endregion

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value) => move = value.Get<Vector2>();
    public void OnLook(InputValue value) { if (cursorInputForLook) look = value.Get<Vector2>(); }
    public void OnJump(InputValue value) => jump = value.isPressed;
    public void OnSprint(InputValue value) => sprint = value.isPressed;
    public void OnDash(InputValue value) => dash = value.isPressed;


    public void OnFire(InputValue value)
    {
        fireReleased = fireHeld && !value.isPressed;
        fireDown = !fireHeld && value.isPressed;
        fireHeld = value.isPressed;
    }

    public bool GetFireInputDown() => fireDown;
    public bool GetFireInputHeld() => fireHeld;
    public bool GetFireInputReleased() => fireReleased;

    public void OnAim(InputValue value)
    {
        if (value.isPressed) aim ^= true; // Toggle aim using XOR
    }

    public void OnCrouch(InputValue value) => crouch = value.isPressed;
    public void OnReload(InputValue value) => reload = value.isPressed;
#endif

    private void Start()
    {
        SetCursorState(cursorLocked); // Ensure cursor is locked at start
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
