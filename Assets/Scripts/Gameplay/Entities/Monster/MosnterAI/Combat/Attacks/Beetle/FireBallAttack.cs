using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.PostProcessing;
using static UnityEngine.UI.GridLayoutGroup;

public class FireballAttack : IMonsterAttack, IProjectilePool
{
    private readonly MonsterBrain _brain;
    private readonly MonsterAnimationController _animationController;
    private readonly MonsterCombat _combat;
    private readonly FireballAttackData _data;
    private readonly MonsterAttackPoints _attackPoints;

    private ObjectPool<Projectile> _pool;
    private float _timer;
    private bool _spawned;
    private bool _finished;
    public bool Finished => _finished;

    public FireballAttack(
        MonsterBrain brain,
        MonsterCombat combat,
        FireballAttackData data,
        MonsterAnimationController animationController,
        MonsterAttackPoints attackPoints)
    {
        Debug.Log(attackPoints);
        _brain = brain;
        _combat = combat;
        _data = data;
        _animationController = animationController;
        _attackPoints = attackPoints;

        if (_data.ProjectilePrefab != null)
        {
            _pool = ObjectPoolingManager.Instance.GetOrCreatePool(_data.ProjectilePrefab);
        }
    }

    public bool CanUse()
    {
        float distance = _brain.DistanceToTarget;

        return distance <= _data.Range;
    }


    public void Begin()
    {
        Debug.Log("Fireball started");

        _timer = 0;
        _spawned = false;
        _finished = false;

        _animationController.PlayFireballAttack();
    }


    public void Tick()
    {
        _timer += Time.deltaTime;


        if (_timer >= 0.7f && !_spawned)
        {
            SpawnFireball();
            _spawned = true;
        }


        if (_timer >= 1.5f)
        {
            _finished = true;
        }
    }


    private void SpawnFireball()
    {
        if (_pool != null)
        {
            Vector3 releasePosition = _attackPoints.FireballSpawnPoint.position;
            Vector3 aimDirection = (_brain.Target.transform.position - releasePosition).normalized;
            Entity owner = _brain.GetComponent<Monster>();

            Projectile projectile = _pool.Get();
            ProjectileContext context = new(owner, releasePosition, aimDirection, _data.Damage, Vector3.zero);
            projectile.Initialize(_data.ProjectileData, context, this);
            projectile.Launch();
        }
    }


    public void End()
    {
        Debug.Log("Fireball finished");
    }

    public void Release(Projectile projectile)
    {
        _pool.Release(projectile);
    }
}