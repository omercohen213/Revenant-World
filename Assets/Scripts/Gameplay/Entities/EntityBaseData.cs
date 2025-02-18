using DevionGames.StatSystem;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBaseData : ScriptableObject
{
    [Header("Stats")]
    public float MaxHealth = 100f;
    public float HealthRegen = 1f;
    public float Speed = 5f;
    public float AttackDamage = 10f;
    public float AttackSpeed = 1f;
    public float Armor = 5f;

    public int Level;
    public float xpReward;  // Amount of XP this entity gives upon defeat

}
