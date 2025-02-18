using UnityEngine;

[RequireComponent(typeof(MonsterData))]
[RequireComponent(typeof(MonsterHealth))]
public class Monster : Entity
{
    protected override void Start()
    {
        base.Start();
    }
}
