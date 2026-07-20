using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : EntityCombat
{
    private PlayerInput _playerInput;
    private WeaponController _weaponManager;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _weaponManager = GetComponent<WeaponController>();
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
        _weaponManager.HandleWeaponReload();
    }

    public virtual void HandleAttackDown()
    {
    }

    public virtual void HandleAttackHeld()
    {
        if (!CanAttack())
            return;
        _weaponManager.ActiveWeapon.TryAttack();
    }

    public virtual void HandleAttackReleased()
    {
    }
}
