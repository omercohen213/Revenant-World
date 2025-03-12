using System;
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


    [SerializeField] protected float _defaultFov = 60f;
    protected float _currentFov;

    protected virtual void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _inventoryManager = GetComponentInParent<InventoryManager>();
        _player = GetComponentInParent<Player>();
        _animator = GetComponentInParent<Animator>();
        _cameraManager = GetComponentInParent<CameraManager>();
    }


    protected virtual void Update()
    {
    }

    public void Equip()
    {
        throw new System.NotImplementedException();
    }

    public void Unequip()
    {
        throw new System.NotImplementedException();
    }
}

public enum WeaponType
{
    AssaultRifle,
    Sniper,
    Bow,
    Katana
}
