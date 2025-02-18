using UnityEngine;

public abstract class EntityData : MonoBehaviour
{
    public EntityBaseData baseData;
    public float Speed;
    public float AttackDamage;
    public float Armor;
    public float AttackSpeed;

    protected virtual void Start()
    {
        ResetToBaseData();
    }

    // Reset data to base values
    protected virtual void ResetToBaseData()
    {
        Speed = baseData.Speed;
        AttackDamage = baseData.AttackDamage;
        Armor = baseData.Armor;
        AttackSpeed = baseData.AttackSpeed;
    }
}
