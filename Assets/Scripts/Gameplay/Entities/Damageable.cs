using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Damageable : MonoBehaviour
{
    [Range(0, 1)]
    [Tooltip("Multiplier to apply to self damage")]
    public float SelfDamageMultiplier = 0.5f;

    [Required]
    [SerializeField] private GameObject _hitVFX;
    [SerializeField] private float _hitVFXLifetime = 0.3f;

    public Health Health { get; private set; }

    void Awake()
    {
        // find the health component either at the same level, or higher in the hierarchy
        Health = GetComponent<Health>();
        if (!Health)
        {
            Health = GetComponentInParent<Health>();
        }
    }

    public void InflictDamage(float damage, GameObject damageSource)
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
    }

    public void ShowImpacVFX(Vector3 position)
    {
        if (_hitVFX != null)
        {
            GameObject impactVfxInstance = Instantiate(_hitVFX, position, Quaternion.identity, transform);
            Destroy(impactVfxInstance, _hitVFXLifetime);
        }       
    }
}

