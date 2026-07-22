using UnityEngine;

 /// <summary>
 /// Contains all of the base data for the entity
 /// </summary>
public abstract class EntityBaseData : ScriptableObject
{
    public string Name;

    [Header("Stats")]
    public float MaxHealth;
    public float HealthRegen;
    public float Speed;
    public float AttackDamage;
    public float AttackSpeed;
    public float Armor;
}
