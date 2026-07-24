using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Damageable))]
public class DamageImpactVFX : MonoBehaviour
{
    private Damageable _damageable;

    [SerializeField] private float _hitVFXLifetime = 0.3f;

    private void Awake()
    {
        _damageable = GetComponent<Damageable>();
    }


    private void OnEnable()
    {
        _damageable.OnHit += SpawnVFX;
    }


    private void OnDisable()
    {
        _damageable.OnHit -= SpawnVFX;
    }


    private void SpawnVFX(HitData hitData)
    {
        GameObject hitVFXPrefab = hitData.HitZoneData.HitVFX;

        if (hitVFXPrefab == null)
            return;


        ObjectPool<GameObject> pool =
            ObjectPoolingManager.Instance.GetOrCreatePool(hitVFXPrefab);


        GameObject hitVFX = pool.Get();

        hitVFX.transform.position = hitData.HitPoint;
        hitVFX.transform.rotation = Quaternion.LookRotation(hitData.HitPoint);


        foreach (var ps in hitVFX.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Clear();
            ps.Play();
        }


        ImpactVFXManager.Instance.ReleaseAfterTime(
            hitVFX,
            _hitVFXLifetime
        );
    }
}