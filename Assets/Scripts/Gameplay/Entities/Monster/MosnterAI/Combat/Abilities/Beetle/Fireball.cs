using GLTF.Schema;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Fireball : AnimationDrivenAbility<FireballAbilityData>, IProjectilePool
{
    private readonly ObjectPool<Projectile> _pool;
    private bool _spawned;

    public Fireball(MonsterAbilitiyContext context, FireballAbilityData data) : base(context, data)
    {
        if (_data.ProjectilePrefab != null)
        {
            _pool = ObjectPoolingManager.Instance.GetOrCreatePool(_data.ProjectilePrefab);
        }
    }

    public override void Begin()
    {
        base.Begin();
        _spawned = false;
    }

    private void SpawnFireball()
    {
        if (_pool == null)
            return;

        Vector3 releasePosition = _context.AttackPoints.FireballSpawnPoint.position;
        Vector3 aimDirection = (_context.TargetContext.CurrentPosition - releasePosition).normalized;
        Entity owner = _context.Owner;

        Projectile projectile = _pool.Get();
        ProjectileContext context = new(owner, releasePosition, aimDirection, _data.Damage, Vector3.zero);
        projectile.Initialize(_data.ProjectileData, context, this);
        projectile.Launch();
    }

    public override void End()
    {
        base.End();
        _context.AnimController.SetAnimationSpeed(1f);
    }

    public void ReleaseFromPool(Projectile projectile)
    {
        _pool.Release(projectile);
    }

    public override void OnAnimationEvent(AbilityAnimationEvent eventType)
    {
        base.OnAnimationEvent(eventType);
        switch (eventType)
        {
            case AbilityAnimationEvent.Release:
                if (_context.TargetContext.HasTarget)
                {
                    // Prevent duplicate release events from spawning multiple projectiles.
                    if (_spawned)
                        return;
                    SpawnFireball();
                    _spawned = true;
                }
                break;
        }
    }

    public override void Cancel()
    {
        base.Cancel();
        _spawned = false;
    }
}