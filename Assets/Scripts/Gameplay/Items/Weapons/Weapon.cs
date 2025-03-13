using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Pool;


public abstract class Weapon : Item, IWeapon
{
    [HideInInspector] public WeaponData WeaponData => ItemData as WeaponData;
    [HideInInspector] public GameEntity Owner;
    public ObjectPool<Projectile> ProjectilePool;

    [Header("Projectile Spawning")]
    public Transform WeaponTip; //Tip of the weapon, where the projectiles are shot
    [HideInInspector] public Vector3 WeaponVelocity; // Current velocity of the weapon object

    [Header("Misc")]
    [SerializeField] protected WeaponType _weaponType;
    [SerializeField] protected Transform _defaultWeaponPosition;
    [SerializeField] protected Crosshair _crosshair;
    [SerializeField] protected float _defaultFov = 60f;

    protected Player _player;
    protected PlayerInput _playerInput;
    protected InventoryManager _inventoryManager;
    protected CameraManager _cameraManager;
    protected Animator _animator;
    protected float _currentFov;

    protected virtual void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _inventoryManager = GetComponentInParent<InventoryManager>();
        _player = GetComponentInParent<Player>();
        _animator = GetComponentInParent<Animator>();
        _cameraManager = GetComponentInParent<CameraManager>();

        if (WeaponData.ProjectilePrefab != null)
        {
            ProjectilePool = ObjectPoolingManager.Instance.GetOrCreatePool(WeaponData.ProjectilePrefab);
        }
        else
        {
            Debug.Log("Projectile prefab not assigned");
        }
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

    // Spawn a projectile
    protected virtual void SpawnProjectile(Vector3 direction)
    {
        if (ProjectilePool != null) {
            Projectile newProjectile = ProjectilePool.Get();
            // Calculate the correct rotation
            Quaternion baseRotation = Quaternion.LookRotation(direction);

            // Apply an additional offset if needed (change the values as necessary)
            Quaternion rotationOffset = Quaternion.Euler(0, 90, 0); // Example offset if the forward is wrong

            // Set position and corrected rotation
            newProjectile.transform.SetPositionAndRotation(WeaponTip.position, baseRotation * rotationOffset);
            //newProjectile.transform.SetPositionAndRotation(WeaponTip.position, Quaternion.LookRotation(direction));
            newProjectile.Shoot(this);
        }
    }
}

public enum WeaponType
{
    AssaultRifle,
    Sniper,
    Bow,
    Katana
}
