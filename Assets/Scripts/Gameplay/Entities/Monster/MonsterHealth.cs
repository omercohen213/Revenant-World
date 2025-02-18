using UnityEngine;

public class MonsterHealth : Health
{
    protected override void Start()
    {
        MaxHealth = GetComponent<MonsterData>().baseData.MaxHealth;
        base.Start();
    }
}
