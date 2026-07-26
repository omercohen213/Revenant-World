using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour
{
    private PlayerInput _playerInput;
    private WeaponController _weaponController;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _weaponController = GetComponent<WeaponController>();
    }

    protected virtual void OnEnable()
    {
        _playerInput.OnReloadPressed += HandleReloadPressed;
        _playerInput.OnAttackDown += HandleAttackDown;
        _playerInput.OnAttackHeld += HandleAttackHeld;
        _playerInput.OnAttackReleased += HandleAttackReleased;
    }

    protected virtual void OnDisable()
    {
        _playerInput.OnReloadPressed -= HandleReloadPressed;
        _playerInput.OnAttackDown -= HandleAttackDown;
        _playerInput.OnAttackHeld -= HandleAttackHeld;
        _playerInput.OnAttackReleased -= HandleAttackReleased;
    }

    private void HandleReloadPressed()
    {
        _weaponController.HandleWeaponReload();
    }

    public virtual void HandleAttackDown()
    {
    }

    public virtual void HandleAttackHeld()
    {
        _weaponController.ActiveWeapon.TryAttack();
    }

    public virtual void HandleAttackReleased()
    {
    }
}
