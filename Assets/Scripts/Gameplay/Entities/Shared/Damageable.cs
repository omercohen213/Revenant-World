using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(EntityHealth))]
public class Damageable : MonoBehaviour
{
    [Range(0, 1)]
    [Tooltip("Multiplier to apply to self damage")]
    public float SelfDamageMultiplier = 0.5f;

    [Required]
    [SerializeField] private GameObject _hitVFX;
    [SerializeField] private float _hitVFXLifetime = 0.3f;
    [SerializeField] private ObjectPool<GameObject> _hitVFXPool;

    public EntityHealth Health { get; private set; }

    void Awake()
    {
        // find the health component either at the same level, or higher in the hierarchy
        Health = GetComponent<EntityHealth>();
        if (!Health)
        {
            Health = GetComponentInParent<EntityHealth>();
        }

    }

    private void Start()
    {
        if (_hitVFX == null)
        {
            Debug.LogError("Hit VFX is not initialized properly.");
            return;
        }
        _hitVFXPool = ObjectPoolingManager.Instance.GetOrCreatePool(_hitVFX);
    }

    // Call the damage handling of health component
    public void InflictDamage(Vector3 hitPoint, float damage, GameObject damageSource)
    {
        if (Health)
        {
            var totalDamage = damage;

            // potentially reduce damages if inflicted by self
            if (Health.gameObject == damageSource)
            {
                totalDamage *= SelfDamageMultiplier;
            }

            // apply the damages
            Health.TakeDamage(totalDamage, damageSource);
        }
        ShowImpactVFX(hitPoint);
    }

    public void ShowImpactVFX(Vector3 position)
    {
        _hitVFX = _hitVFXPool.Get();
        _hitVFX.transform.position = position;
        _hitVFX.transform.SetParent(transform);

        // Restart particle effects
        foreach (var ps in _hitVFX.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Clear();
            ps.Play();
        }

        // Start a coroutine on an active object (not this object)
        ImpactVFXManager.Instance.ReleaseAfterTime(_hitVFX, _hitVFXLifetime);

    }
}

