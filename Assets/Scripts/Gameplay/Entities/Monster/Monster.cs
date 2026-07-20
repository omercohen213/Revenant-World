using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(MonsterDataManager))]
[RequireComponent(typeof(Health))]
public class Monster : Entity
{
    public MonsterDataManager MonsterData;
    private ObjectPool<Monster> _monsterPool;

    protected override void Awake()
    {
        base.Awake();
        MonsterData = GetComponent<MonsterDataManager>();
    }

    protected override void Start()
    {
        base.Start();
        _monsterPool = ObjectPoolingManager.Instance.GetOrCreatePool(this);
    }

    protected override void HandleDeath(Health health, GameObject killer)
    {
        base.HandleDeath(health, killer);
        _monsterPool.Release(this);
    }

    public override EntityRuntimeData GetEntityData()
    {
        return MonsterData;
    }
}
