using UnityEngine;

 /// <summary>
 /// Contains all of the base data for the entity
 /// </summary>
public abstract class GameEntityBaseData : ScriptableObject
{
    public string Name;
    // public EntityType EntityType

    [Header("Stats")]
    public float MaxHealth;
    public float HealthRegen;
    public float Speed;
    public float AttackDamage;
    public float AttackSpeed;
    public float Armor;

    [Header("Kill Rewards")]
    public int XpReward; 
    public int KpReward;  
}
