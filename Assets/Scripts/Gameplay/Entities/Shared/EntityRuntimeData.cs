using UnityEngine;

/// <summary>
/// Contains all runtime data shared by every entity.
/// </summary>
public abstract class EntityRuntimeData : MonoBehaviour
{
    // Untyped access for generic systems.

    public abstract EntityBaseData BaseDataUntyped { get; }

    [Header("Runtime Stats")]
    public int Level; 
    public int MaxHealth;
    public int Health;
    public float MovementSpeed;
    public float AttackDamage;
    public float AttackSpeed;
    public float Armor;
    public int XpReward;
    public int KpReward;
}


public abstract class EntityRuntimeData<TBaseData> : EntityRuntimeData
    where TBaseData : EntityBaseData
{
    [SerializeField]
    private TBaseData _baseData;

    // Strongly-typed access for derived classes.
    public TBaseData BaseData => _baseData;

    // Untyped access for shared systems.
    public override EntityBaseData BaseDataUntyped => _baseData;

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
        Level = 1;
        MovementSpeed = BaseData.MovementSpeed;
        AttackDamage = BaseData.AttackDamage;
        Armor = BaseData.Armor;
    }
}
