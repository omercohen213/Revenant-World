using UnityEngine;

/// <summary>
/// Contains all of the data for the entity.
/// </summary>
public abstract class EntityDataManager : MonoBehaviour
{
    public EntityBaseData baseData;

    public int Level;

    public float Speed;
    public float AttackDamage;
    public float Armor;
    public float AttackSpeed;
    public int XpReward;
    public int ScoreReward;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        ResetToBaseData();
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {

    }

    // Reset data to base values
    protected virtual void ResetToBaseData()
    {
        Level = 1;
        Speed = baseData.Speed;
        AttackDamage = baseData.AttackDamage;
        Armor = baseData.Armor;
        AttackSpeed = baseData.AttackSpeed;
        XpReward = baseData.XpReward;
    }
}
