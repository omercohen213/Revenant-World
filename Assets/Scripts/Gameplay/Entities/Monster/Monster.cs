using UnityEngine;

[RequireComponent(typeof(MonsterDataManager))]
[RequireComponent(typeof(Health))]
public class Monster : GameEntity
{
    public MonsterDataManager MonsterData;

    protected override void Awake()
    {
        base.Awake();
        MonsterData = GetComponent<MonsterDataManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override GameEntityDataManager GetEntityData()
    {
        return MonsterData;
    }
}
