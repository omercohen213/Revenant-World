using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
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

    Vector3 m_LastRootPosition;
    Vector3 m_Velocity;
    bool _hasTrajectoryOverride;
    float _shootTime;
    Vector3 _trajectoryCorrectionVector;
    Vector3 _consumedTrajectoryCorrectionVector;
    List<Collider> m_IgnoredColliders;

    const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

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

    public void Shoot(Weapon weapon)
    {
        Owner = weapon.Owner;
        Transform muzzleTransform = weapon.WeaponMuzzle;

        // Get the active camera
        PlayerWeaponManager playerWeaponManager = Owner.GetComponent<PlayerWeaponManager>();
        CinemachineCamera activeCamera = playerWeaponManager.IsAiming ? playerWeaponManager.AimCamera : playerWeaponManager.DefaultCamera;

        // Get aiming direction from the correct camera
        Vector3 aimDirection = activeCamera.transform.forward;

        // Correct the bullet rotation by aligning it with the aim direction
        Quaternion correctedRotation = Quaternion.LookRotation(aimDirection);

        // Apply the corrected rotation
        //transform.rotation = correctedRotation;
        transform.position = muzzleTransform.position;

        // Store the initial direction
        InitialPosition = transform.position;
        InitialDirection = aimDirection;

        // Set the velocity in the new direction
        m_Velocity = aimDirection * Speed;

        // Apply inherited weapon velocity
        InheritedMuzzleVelocity = weapon.MuzzleWorldVelocity;
        if (InheritWeaponVelocity)
        {
            m_Velocity += InheritedMuzzleVelocity;
        }

        // Ignore colliders of the weapon owner
        Collider[] ownerColliders = Owner.GetComponentsInChildren<Collider>();
        m_IgnoredColliders = new List<Collider>(ownerColliders);

        // Trigger the shoot event
        OnShoot?.Invoke();
    }

    void Update()
    {
        // Move
        transform.position += m_Velocity * Time.deltaTime;
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
            float distanceThisFrame = (Root.position - m_LastRootPosition).magnitude;
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

        // Orient towards velocity
        transform.forward = m_Velocity.normalized;

        // Gravity
        if (GravityDownAcceleration > 0)
        {
            // add gravity to the projectile velocity for ballistic effect
            m_Velocity += Vector3.down * GravityDownAcceleration * Time.deltaTime;
        }

        // Hit detection
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            // Sphere cast
            Vector3 displacementSinceLastFrame = Tip.position - m_LastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(m_LastRootPosition, Radius,
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

        m_LastRootPosition = Root.position;
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
        if (m_IgnoredColliders != null && m_IgnoredColliders.Contains(hit.collider))
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
