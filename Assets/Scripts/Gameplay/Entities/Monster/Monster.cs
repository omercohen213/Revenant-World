using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(MonsterRuntimeData))]
[RequireComponent(typeof(EntityHealth))]
public class Monster : Entity
{
    public MonsterRuntimeData MonsterData;
    private ObjectPool<Monster> _monsterPool;

    protected override void Awake()
    {
        base.Awake();
        MonsterData = GetComponent<MonsterRuntimeData>();
    }

    protected override void Start()
    {
        base.Start();
        _monsterPool = ObjectPoolingManager.Instance.GetOrCreatePool(this);
    }

    protected override void HandleDeath(EntityHealth health, GameObject killer)
    {
        base.HandleDeath(health, killer);
        _monsterPool.Release(this);
    }

    public override EntityRuntimeData GetEntityData()
    {
        return MonsterData;
    }
}
