using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Radius of this projectile's collision detection")]
    public float Radius = 0.01f;

    [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
    public Transform Root;

    [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
    public Transform Tip;

    [Tooltip("LifeTime of the projectile")]
    public float MaxLifeTime = 5f;

    [Tooltip("Default VFX prefab to spawn upon impact on objects")]
    public GameObject DefaultImpactVfx;
    private ObjectPool<GameObject> _impactVFXPool;

    [Tooltip("LifeTime of the VFX before being destroyed")]
    public float ImpactVfxLifetime = 5f;

    [Tooltip("Offset along the hit normal where the VFX will be spawned")]
    public float ImpactVfxSpawnOffset = 0.1f;

    [Tooltip("Layers this projectile can collide with")]
    public LayerMask HittableLayers = -1;

    [Header("Movement")]
    [Tooltip("Speed of the projectile")]
    public float Speed = 20f;

    [Tooltip("Downward acceleration from gravity")]
    public float GravityDownAcceleration = 0f;


    [Tooltip(
        "Distance over which the projectile will correct its course to fit the intended trajectory (used to drift projectiles towards center of screen in First Person view). At values under 0, there is no correction")]
    public float TrajectoryCorrectionDistance = -1;

    [Tooltip("Determines if the projectile inherits the velocity that the weapon's muzzle had when firing")]
    public bool InheritWeaponVelocity = false;


    private Vector3 _lastRootPosition;
    private Vector3 _velocity;
    private bool _hasTrajectoryOverride;
    private Vector3 _trajectoryCorrectionVector;
    private Vector3 _consumedTrajectoryCorrectionVector;
    private List<Collider> _ignoredColliders;

    private const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

    public GameObject Owner { get; private set; }
    public Weapon WeaponParent { get; private set; }
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }
    public Vector3 InheritedMuzzleVelocity { get; private set; }

    private void Awake()
    {
        if (DefaultImpactVfx != null)
        {
            _impactVFXPool = ObjectPoolingManager.Instance.GetOrCreatePool(DefaultImpactVfx);
        }
    }

    void OnEnable()
    {
        ResetState();
        StartCoroutine(SelfDestructCoroutine(MaxLifeTime));
    }

    public void ResetState()
    {
        _velocity = Vector3.zero;
        _ignoredColliders?.Clear();
        _hasTrajectoryOverride = false;
        _consumedTrajectoryCorrectionVector = Vector3.zero;
        _lastRootPosition = Root.position;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        MoveProjectile();
        CorrectTrajectory();
        HandleRotation();
        HandleGravity();
        HandleHitDetection();

        _lastRootPosition = Root.position;
    }


    private void MoveProjectile()
    {
        transform.position += _velocity * Time.deltaTime;
        if (InheritWeaponVelocity)
        {
            transform.position += InheritedMuzzleVelocity * Time.deltaTime;
        }
    }

    // Drift towards trajectory override (this is so that projectiles can be centered 
    // with the camera center even though the actual weapon is offset)
    private void CorrectTrajectory()
    {
        if (_hasTrajectoryOverride && _consumedTrajectoryCorrectionVector.sqrMagnitude <
                    _trajectoryCorrectionVector.sqrMagnitude)
        {
            Vector3 correctionLeft = _trajectoryCorrectionVector - _consumedTrajectoryCorrectionVector;
            float distanceThisFrame = (Root.position - _lastRootPosition).magnitude;
            Vector3 correctionThisFrame =
                (distanceThisFrame / TrajectoryCorrectionDistance) * _trajectoryCorrectionVector;
            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
            _consumedTrajectoryCorrectionVector += correctionThisFrame;

            // Detect end of correction
            if (_consumedTrajectoryCorrectionVector.sqrMagnitude == _trajectoryCorrectionVector.sqrMagnitude)
            {
                _hasTrajectoryOverride = false;
            }

            transform.position += correctionThisFrame;
        }
    }

    // Maintain correct rotation (Prevents mid-flight rotation issues)
    private void HandleRotation()
    {
        if (_velocity.sqrMagnitude > 0.01f)  // Only rotate if moving
        {
            transform.rotation = Quaternion.LookRotation(_velocity * -1);
        }
    }

    // add gravity to the projectile velocity for ballistic effect
    private void HandleGravity()
    {
        if (GravityDownAcceleration > 0)
        {
            _velocity += Vector3.down * GravityDownAcceleration * Time.deltaTime;
        }
    }

    // Hit detection by casting a sphere
    private void HandleHitDetection()
    {
        RaycastHit closestHit = new RaycastHit();
        closestHit.distance = Mathf.Infinity;
        bool foundHit = false;

        // Sphere cast
        Vector3 displacementSinceLastFrame = Tip.position - _lastRootPosition;
        RaycastHit[] hits = Physics.SphereCastAll(_lastRootPosition, Radius,
            displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers,
            k_TriggerInteraction);
        foreach (var hit in hits)
        {
            if (IsHitValid(hit) && hit.distance < closestHit.distance)
            {
                foundHit = true;
                closestHit = hit;
            }
        }

        if (foundHit)
        {
            // Handle case of casting while already inside a collider
            if (closestHit.distance <= 0f)
            {
                closestHit.point = Root.position;
                closestHit.normal = -transform.forward;
            }

            OnHit(closestHit.point, closestHit.normal, closestHit.collider);
        }
    }


    public void Shoot(Weapon weapon)
    {
        WeaponParent = weapon;
        Owner = weapon.Owner.gameObject;
        Transform muzzleTransform = weapon.WeaponTip;

        CameraManager ownerCameraManager = Owner.GetComponent<CameraManager>();

        CinemachineCamera activeCamera = ownerCameraManager.Camera;

        // Get aiming direction from the correct camera
        Vector3 aimDirection = activeCamera.transform.forward;

        // Set projectile position at the muzzle   
        // Correct rotation: Align projectile's forward (Z-axis) with the aiming direction
        transform.SetPositionAndRotation(muzzleTransform.position, Quaternion.LookRotation(aimDirection));

        // Store the initial direction
        InitialPosition = transform.position;
        InitialDirection = aimDirection;

        // Set the velocity in the new direction
        _velocity = aimDirection * Speed;

        // Apply inherited weapon velocity
        InheritedMuzzleVelocity = weapon.WeaponVelocity;
        if (InheritWeaponVelocity)
        {
            _velocity += InheritedMuzzleVelocity;
        }

        // Ignore colliders of the weapon owner
        Collider[] ownerColliders = Owner.GetComponentsInChildren<Collider>();
        _ignoredColliders = new List<Collider>(ownerColliders);
    }

    private bool IsHitValid(RaycastHit hit)
    {
        if (hit.distance == 0) return false;

        // ignore hits with an ignore component
        if (hit.collider.GetComponent<IgnoreHitDetection>())
        {
            return false;
        }

        // ignore hits with specific ignored colliders (self colliders, by default)
        if (_ignoredColliders != null && _ignoredColliders.Contains(hit.collider))
        {
            return false;
        }

        return true;
    }

    private void OnHit(Vector3 hitPoint, Vector3 normal, Collider collider)
    {
        // damage
        Damageable damageable = collider.GetComponent<Damageable>();
        if (damageable)
        {
            HitData hitData = new()
            {
                HitPoint = hitPoint,
                Damage = WeaponParent.WeaponData.ProjectileDamage,
                DamageSource = Owner
            };

            damageable.ReceiveHit(hitData);
        }

        else
        {
            // impact vfx on objects
            if (DefaultImpactVfx != null)
            {
                GameObject impactVFX = _impactVFXPool.Get();
                impactVFX.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(normal));

                // Start a coroutine on an active object (not the projectile)
                ImpactVFXManager.Instance.ReleaseAfterTime(impactVFX, 1f);
            }
        }

        // return the projectile to the pool
        WeaponParent.ProjectilePool.Release(this);
    }

    private IEnumerator SelfDestructCoroutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        WeaponParent.ProjectilePool.Release(this);
    }
}
