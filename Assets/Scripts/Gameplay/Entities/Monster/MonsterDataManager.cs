using System.Collections.Generic;
using UnityEngine;

public class MonsterDataManager : EntityDataManager
{
    public float aggroRange;         // Range at which monster targets players
    public float attackPatternCooldown;  // Cooldown between special attack patterns
    public float aiStateChangeRate;  // Rate at which monster switches behavior (e.g., idle to attacking)
    public bool isFlying;            // True if monster can fly, affecting movement behavior
    public List<string> specialAbilities;  // Unique abilities or attacks for the monster

    // Specific loot or rewards dropped by this monster
    public LootTable lootTable;  // A list of item names this monster can drop
}
