using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    #region Character Input Values
    [Header("Character Input Values")]
    public Vector2 Move;
    public Vector2 Look;
    public bool Jump;
    public bool Sprint;
    public bool Dash;

    public bool FireDown;
    public bool FireHeld;
    public bool FireReleased;
    public bool AimDown;
    public bool Crouch;
    public bool Reload;
    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value) => Move = value.Get<Vector2>();
    public void OnLook(InputValue value) { Look = value.Get<Vector2>(); Debug.Log("ss"); }
    public void OnJump(InputValue value) => Jump = value.isPressed;
    public bool GetJumpInputDown()
    {
        // Capture the current jump state and reset it so it only lasts one frame.
        bool wasPressed = Jump;
        Jump = false;
        return wasPressed;
    }
    public void OnSprint(InputValue value) => Sprint = value.isPressed;
    public void OnDash(InputValue value) => Dash = value.isPressed;
    public bool GetDashInputDown()
    {
        // Capture the current jump state and reset it so it only lasts one frame.
        bool wasPressed = Dash;
        Dash = false;
        return wasPressed;
    }
    public void OnFire(InputValue value)
    {
        FireReleased = FireHeld && !value.isPressed;
        FireDown = !FireHeld && value.isPressed;
        FireHeld = value.isPressed;
    }
    public bool GetFireInputDown() => FireDown;
    public bool GetFireInputHeld() => FireHeld;
    public bool GetFireInputReleased() => FireReleased;
    public void OnAim(InputValue value) => AimDown = value.isPressed;
    public bool GetAimInputDown()
    {
        // Capture the current aimDown state and reset it so it only lasts one frame.
        bool wasPressed = AimDown;
        AimDown = false;
        return wasPressed;
    }
    public void OnCrouch(InputValue value) => Crouch = value.isPressed;
    public void OnReload(InputValue value) => Reload = value.isPressed;
#endif

}
