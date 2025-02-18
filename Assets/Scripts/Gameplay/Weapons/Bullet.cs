using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
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

    [Tooltip("VFX prefab to spawn upon impact")]
    public GameObject ImpactVfx;

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

    [Header("Damage")]
    [Tooltip("Damage of the projectile")]
    public float Damage = 40f;

    [Header("Debug")]
    [Tooltip("Color of the projectile radius debug view")]
    public Color RadiusColor = Color.cyan * 0.2f;

    private Vector3 _lastRootPosition;
    private Vector3 _velocity;
    private bool _hasTrajectoryOverride;
    private float _shootTime;
    private Vector3 _trajectoryCorrectionVector;
    private Vector3 _consumedTrajectoryCorrectionVector;
    private List<Collider> _ignoredColliders;

    private const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

    public GameObject Owner { get; private set; }
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }
    public Vector3 InheritedMuzzleVelocity { get; private set; }

    public UnityAction OnShoot;

    void OnEnable()
    {
        Destroy(gameObject, MaxLifeTime);
        OnShoot?.Invoke();
    }

    public void Shoot(Gun gun)
    {
        Owner = gun.Owner.gameObject;
        Transform muzzleTransform = gun.WeaponMuzzle;

        if (!DebugUtil.SafeGetComponent(Owner, out WeaponManager playerWeaponManager)) return;
        
        // Get the active camera
        CinemachineCamera activeCamera = playerWeaponManager.Camera;

        // Get aiming direction from the correct camera
        Vector3 aimDirection = activeCamera.transform.forward;

        // Set bullet position at the muzzle   
        // Correct rotation: Align bullet's forward (Z-axis) with the aiming direction
         transform.SetPositionAndRotation(muzzleTransform.position, Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90f, 0f, 0f));

        // Store the initial direction
        InitialPosition = transform.position;
        InitialDirection = aimDirection;

        // Set the velocity in the new direction
        _velocity = aimDirection * Speed;

        // Apply inherited weapon velocity
        InheritedMuzzleVelocity = gun.MuzzleWorldVelocity;
        if (InheritWeaponVelocity)
        {
            _velocity += InheritedMuzzleVelocity;
        }

        // Ignore colliders of the weapon owner
        Collider[] ownerColliders = Owner.GetComponentsInChildren<Collider>();
        _ignoredColliders = new List<Collider>(ownerColliders);

        // Trigger the shoot event
        OnShoot?.Invoke();
    }

    void Update()
    {
        // Move
        transform.position += _velocity * Time.deltaTime;
        if (InheritWeaponVelocity)
        {
            transform.position += InheritedMuzzleVelocity * Time.deltaTime;
        }

        // Drift towards trajectory override (this is so that projectiles can be centered 
        // with the camera center even though the actual weapon is offset)
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

        /*// Orient towards velocity
        transform.forward = _velocity.normalized;*/

        // Maintain correct rotation (Prevents mid-flight rotation issues)
        if (_velocity.sqrMagnitude > 0.01f)  // Only rotate if moving
        {
            transform.rotation = Quaternion.LookRotation(_velocity) * Quaternion.Euler(90f, 0f, 0f);
        }

        // Gravity
        if (GravityDownAcceleration > 0)
        {
            // add gravity to the projectile velocity for ballistic effect
            _velocity += Vector3.down * GravityDownAcceleration * Time.deltaTime;
        }

        // Hit detection
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

        _lastRootPosition = Root.position;
    }

    bool IsHitValid(RaycastHit hit)
    {
        // ignore hits with an ignore component
        if (hit.collider.GetComponent<IgnoreHitDetection>())
        {
            return false;
        }

        // ignore hits with triggers that don't have a Damageable component
        if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
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

    void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        // damage
        // point damage
        Damageable damageable = collider.GetComponent<Damageable>();
        if (damageable)
        {
            damageable.InflictDamage(Damage, false, Owner);
        }


        // impact vfx
        if (ImpactVfx)
        {
            GameObject impactVfxInstance = Instantiate(ImpactVfx, point + (normal * ImpactVfxSpawnOffset),
                Quaternion.LookRotation(normal));
            if (ImpactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance.gameObject, ImpactVfxLifetime);
            }
        }

        // Self Destruct
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = RadiusColor;
        Gizmos.DrawSphere(transform.position, Radius);
    }
}
