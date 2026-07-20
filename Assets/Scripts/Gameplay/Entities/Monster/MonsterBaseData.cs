using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseData", menuName = "Scriptable Objects/MonsterBaseData")]
public class MonsterBaseData : EntityBaseData
{
    [Header("Detection")]
    public float aggroRange;         // Range at which monster targets players
    public float loseTargetRange;

    [Header("Combat")]
    public float attackPatternCooldown;  // Cooldown between special attack patterns
    public float aiStateChangeRate;  // Rate at which monster switches behavior (e.g., idle to attacking)
    public float attackRange;  // Rate at which monster switches behavior (e.g., idle to attacking)

    [Header("Movement")]
    //public bool canFly;       
    public float rotationSpeed;

    [Header("Loot")]
    public LootTable lootTable;  // A list of item names this monster can drop
}
