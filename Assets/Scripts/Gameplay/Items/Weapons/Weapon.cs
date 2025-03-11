using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


public abstract class Weapon : Item, IWeapon
{
    public GameEntity Owner;

    protected Player _player;
    protected PlayerInput _playerInput;
    protected InventoryManager _inventoryManager;
    protected CameraManager _cameraManager;
    protected Animator _animator;

    [SerializeField] protected WeaponType _weaponType;
    [SerializeField] protected Transform _defaultWeaponPosition;
    [SerializeField] protected Crosshair _crosshair;


    [SerializeField] protected bool IsReloading = false;
    [SerializeField] protected float _defaultFov = 60f;
    protected float _currentFov;


    protected virtual void Awake()
    {
        if (!DebugUtil.SafeGetComponentInParent(gameObject, out _playerInput)) return;
        if (!DebugUtil.SafeGetComponentInParent(gameObject, out _inventoryManager)) return;
        if (!DebugUtil.SafeGetComponentInParent(gameObject, out _player)) return;

        _animator = GetComponentInParent<Animator>();
        _cameraManager = GetComponentInParent<CameraManager>();
    }

    public void Equip()
    {
        throw new System.NotImplementedException();
    }

    public void Unequip()
    {
        throw new System.NotImplementedException();
    }

    public virtual void HandleActions()
    {
        
    }
}

public enum WeaponType
{
    AssaultRifle,
    Sniper,
    Bow,
    Katana
}
