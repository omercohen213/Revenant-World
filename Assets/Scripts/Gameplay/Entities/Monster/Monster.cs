using Micosmo.SensorToolkit;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

[RequireComponent(typeof(MonsterRuntimeData))]
[RequireComponent(typeof(EntityHealth))]
public class Monster : Entity
{
    private MonsterRuntimeData _data;
    private MonsterAnimationController _animController;
    private MonsterAttackPoints _attackPoints;
    private PatrolArea _patrolArea;
    private LOSSensor _sensor;

    private ObjectPool<Monster> _monsterPool;
    private MonsterComposition _composition;

    public MonsterRuntimeData MonsterData { get => _data; }

    protected override void Awake()
    {
        base.Awake();
        _data = GetComponent<MonsterRuntimeData>(); // remove after entity health refactor
         
        MonsterReferences references = CreateReferences();
        _composition = new MonsterComposition(references);
        _composition.Build();

    }
    private MonsterReferences CreateReferences()
    {
        MonsterReferences references = new(
        this,
        GetComponent<MonsterRuntimeData>(),
        GetComponent<NavMeshAgent>(),
        GetComponent<LOSSensor>(),
        GetComponent<MonsterAttackPoints>(),
        GetComponentInChildren<MonsterAnimationController>(),
        GetComponentInChildren<MonsterAnimationEvents>(),
        GetComponent<PatrolArea>()
    );

        return references;
    }


    protected override void Start()
    {
        base.Start();
        _monsterPool = ObjectPoolingManager.Instance.GetOrCreatePool(this);
    }

    protected override void Update()
    {
        base.Update();
        _composition.Tick(Time.deltaTime);
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


    protected override void OnDestroy()
    {
        base.OnDestroy();
        _composition.Dispose();
    }

    private void OnDrawGizmosSelected()
    {
        MonsterRuntimeData runtimeData = GetComponent<MonsterRuntimeData>();

        foreach (MonsterAbilityData ability in runtimeData.BaseData.Abilities)
        {
            Gizmos.color = ability.DebugColor;
            Gizmos.DrawWireSphere(transform.position, ability.Range);
        }
    }
}
