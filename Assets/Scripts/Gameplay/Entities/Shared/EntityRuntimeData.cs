using UnityEngine;

/// Contains all of the data for the entity; this data can be changed during the game

public abstract class EntityRuntimeData : MonoBehaviour
{
    public abstract EntityBaseData BaseData { get; }

    public int CurrentLevel;
    public int CurrentMaxHealth;
    public int CurrentHealth;
    public float CurrentMovementSpeed;
    public float CurrentAttackDamage;
    public float CurrentAttackSpeed;    
    public float CurrentArmor;
    public int CurrentXpReward;
    public int CurrentKpReward;
}


public abstract class EntityRuntimeData<TBaseData> : EntityRuntimeData
    where TBaseData : EntityBaseData
{
    [SerializeField]
    private TBaseData _baseData;

    public override EntityBaseData BaseData => _baseData;

    protected TBaseData TypedBaseData => _baseData;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        ResetToBaseData();
    }

    // Reset data to base values
    protected virtual void ResetToBaseData()
    {
        CurrentLevel = 1;
        CurrentMovementSpeed = BaseData.Speed;
        CurrentAttackDamage = BaseData.AttackDamage;
        CurrentArmor = BaseData.Armor;
    }
}
