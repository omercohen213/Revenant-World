using NaughtyAttributes;
using System;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;


public abstract class Weapon : Item, IWeapon, IProjectilePool
{
    [HideInInspector] public WeaponData WeaponData => ItemData as WeaponData;
    [HideInInspector] public Entity Owner;
    public ObjectPool<Projectile> ProjectilePool;

    [Header("Projectile Spawning")]
    public Transform WeaponTip; //Tip of the weapon, where the projectiles are shot
    [HideInInspector] public Vector3 WeaponVelocity; // Current velocity of the weapon object

    [Header("Misc")]
    [Layer][SerializeField] private LayerMask _defaultWeaponLayer;
    [SerializeField] protected WeaponType _weaponType;
    [SerializeField] protected float _defaultFov = 60f;

    protected Player _player;
    protected PlayerInput _playerInput;
    protected InventoryManager _inventoryManager;
    protected CameraManager _cameraManager;
    protected Animator _animator;
    protected float _currentFov;
    protected Crosshair _crosshair;
    public Crosshair Crosshair => _crosshair;

    protected virtual void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _inventoryManager = GetComponentInParent<InventoryManager>();
        _player = GetComponentInParent<Player>();
        _animator = GetComponentInParent<Animator>();
        _cameraManager = GetComponentInParent<CameraManager>();
        _crosshair = _player.GetComponentInChildren<Crosshair>();

        if (WeaponData.ProjectilePrefab != null)
        {
            ProjectilePool = ObjectPoolingManager.Instance.GetOrCreatePool(WeaponData.ProjectilePrefab);
        }
        else
        {
            Debug.Log("Projectile prefab not assigned");
        }
    }


    private void Start()
    {
        if (_player != null)
        {
            Owner = _player;
        }
    }

    protected virtual void Update()
    {
    }

    //
    public void Equip()
    {
        SetLayer(gameObject, _defaultWeaponLayer);
    }

    public void Unequip()
    {
        LayerMask hiddenLayer = LayerMask.NameToLayer("Hidden");
        SetLayer(gameObject, hiddenLayer);
    }

    private void SetLayer(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, newLayer);
        }
    }

    public virtual void TryAttack()
    {

    }

    // Spawn a projectile
    protected virtual void SpawnProjectile(Vector3 direction)
    {
        if (ProjectilePool != null)
        {
            CameraManager ownerCameraManager = Owner.GetComponent<CameraManager>();
            CinemachineCamera activeCamera = ownerCameraManager.Camera;
            Vector3 aimDirection = activeCamera.transform.forward;

            Projectile projectile = ProjectilePool.Get();
            ProjectileContext context = new(Owner, WeaponTip.position, aimDirection, WeaponData.ProjectileDamage, WeaponVelocity);
            projectile.Initialize(WeaponData.ProjectileData, context, this);
            projectile.Launch();
        }
    }

    public void ReleaseFromPool(Projectile projectile)
    {
        ProjectilePool.Release(projectile);
    }
}

public enum WeaponType
{
    AssaultRifle,
    Sniper,
    Bow,
    Katana
}
