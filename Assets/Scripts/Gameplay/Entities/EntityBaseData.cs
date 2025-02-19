using System.Collections.Generic;
using UnityEngine;

 /// <summary>
 /// Contains all of the base data for the entity
 /// </summary>
public abstract class EntityBaseData : ScriptableObject
{
    [Header("Stats")]
    public float MaxHealth = 100f;
    public float HealthRegen = 1f;
    public float Speed = 5f;
    public float AttackDamage = 10f;
    public float AttackSpeed = 1f;
    public float Armor = 5f;

    [Header("Kill Rewards")]
    public int XpReward = 50; 
    public int ScoreReward = 1;  
}
