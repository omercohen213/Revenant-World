using UnityEngine;

/// Contains all of the data for the entity; this data can be changed during the game
public abstract class EntityRuntimeData : MonoBehaviour
{
    public EntityBaseData baseData;

    public int Level;
    public float MovementSpeed;
    public float AttackDamage;
    public float AttackSpeed;
    public float Armor;
    public int XpReward;
    public int KpReward;

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
        MovementSpeed = baseData.Speed;
        AttackDamage = baseData.AttackDamage;
        Armor = baseData.Armor;
        AttackSpeed = baseData.AttackSpeed;
        XpReward = baseData.XpReward;
        KpReward = baseData.KpReward;
    }
}
