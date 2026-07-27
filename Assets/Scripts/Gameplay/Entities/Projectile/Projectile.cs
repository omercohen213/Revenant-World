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
    [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
    public Transform Root;

    [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
    public Transform Tip;

    private IProjectilePool _pool;
    private ObjectPool<GameObject> _impactVFXPool;

    private ProjectileData _data;
    private ProjectileContext _context;
    private bool _initialized;

    private Vector3 _lastRootPosition;
    private Vector3 _velocity;
    private bool _hasTrajectoryOverride;
    private Vector3 _trajectoryCorrectionVector;
    private Vector3 _consumedTrajectoryCorrectionVector;
    private List<Collider> _ignoredColliders;

    private const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }
    public Vector3 InheritedMuzzleVelocity { get; private set; }

    public void Initialize(ProjectileData data, ProjectileContext context, IProjectilePool pool)
    {
        _initialized = true;
        _context = context;
        _pool = pool;

        if (_data != data)
        {
            _data = data;
            SetupData();
        }
    }

    private void SetupData()
    {
        if (_data.ImpactVFX != null)
        {
            _impactVFXPool = ObjectPoolingManager.Instance.GetOrCreatePool(_data.ImpactVFX);
        }
    }

    public void SetPool(IProjectilePool pool)
    {
        _pool = pool;
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
        if (!_initialized)
            return;

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
        if (_data.InheritWeaponVelocity)
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
                (distanceThisFrame / _data.TrajectoryCorrectionDistance) * _trajectoryCorrectionVector;
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
            transform.rotation = Quaternion.LookRotation(_velocity);
        }
    }

    // add gravity to the projectile velocity for ballistic effect
    private void HandleGravity()
    {
        if (_data.GravityAcceleration > 0)
        {
            _velocity += Vector3.down * _data.GravityAcceleration * Time.deltaTime;
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
        RaycastHit[] hits = Physics.SphereCastAll(_lastRootPosition, _data.Radius,
            displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, _data.HittableLayers,
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

    public void Launch()
    {
        ResetState();

        // Set projectile position at the muzzle   
        // Correct rotation: Align projectile's forward (Z-axis) with the aiming direction
        transform.SetPositionAndRotation(_context.ReleasePosition, Quaternion.LookRotation(_context.Direction));

        // Store the initial direction
        InitialPosition = transform.position;
        InitialDirection = _context.Direction;

        // Set the velocity in the new direction
        _velocity = _context.Direction * _data.Speed;

        // Apply inherited weapon velocity
        InheritedMuzzleVelocity = _context.InitialVelocity;
        if (_data.InheritWeaponVelocity)
        {
            _velocity += InheritedMuzzleVelocity;
        }

        // Ignore colliders of the weapon owner
        Collider[] ownerColliders = _context.Owner.GetComponentsInChildren<Collider>();
        _ignoredColliders = new List<Collider>(ownerColliders);

        gameObject.SetActive(true);
        StartCoroutine(SelfDestructCoroutine(_data.MaxLifeTime));
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
                Damage = _context.Damage,
                DamageSource = _context.Owner.gameObject
            };

            damageable.ReceiveHit(hitData);
        }

        else
        {
            // impact vfx on objects
            if (_data.ImpactVFX != null)
            {
                GameObject impactVFX = _impactVFXPool.Get();
                impactVFX.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(normal));

                // Start a coroutine on an active object (not the projectile)
                ImpactVFXManager.Instance.ReleaseAfterTime(impactVFX, 1f);
            }
        }

        // return the projectile to the pool
        _pool.Release(this);
    }

    private IEnumerator SelfDestructCoroutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        _pool.Release(this);
    }
}
