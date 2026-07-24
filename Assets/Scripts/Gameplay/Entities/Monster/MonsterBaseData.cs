using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseData", menuName = "Scriptable Objects/MonsterBaseData")]
public class MonsterBaseData : EntityBaseData
{
    [Header("States")]
    public float PatrolStoppingDelay;       // Delay time for stopping each time reched destination in patrol state

    [Header("Detection")]
    public float AggroRange;       // Range at which monster targets players
    public float LoseTargetRange;

    [Header("Combat")]
    public float AttackPatternCooldown;     // Cooldown between special attack patterns
    public float AiStateChangeRate;     // Rate at which monster switches behavior (e.g., idle to attacking)
    public float AttackRange;        // Rate at which monster switches behavior (e.g., idle to attacking)

    [Header("Movement")]
    //public bool canFly;       
    public float RotationSpeed;

    [Header("Loot")]
    public LootTable LootTable;    // A list of item names this monster can drop

    [Header("Kill Rewards")]
    public int XpReward;
    public int KpReward;
}
