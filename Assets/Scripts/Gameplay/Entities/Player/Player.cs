using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerDataManager))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InventoryManager))]
public class Player : Entity
{
    public PlayerDataManager PlayerData;
    public Weapon ActiveWeapon;
    public Weapon StartingWeapon;

    private InventoryManager _inventoryManager;

    protected override void Awake()
    {
        base.Awake();
        PlayerData = GetComponent<PlayerDataManager>();
        _inventoryManager = GetComponent<InventoryManager>();
        ActiveWeapon = StartingWeapon;
    }

    protected override void OnEnable()
    {
        PlayerData.OnLevelUp += OnLevelUp;

    }
    protected override void OnDisable()
    {
        PlayerData.OnLevelUp -= OnLevelUp;

    }

    protected override void Start()
    {
        base.Start();
    }

    public void GetRewardForKill(Entity entity)
    {
        PlayerData.AddXP(entity.GetEntityData().XpReward);
        PlayerData.AddScore(entity.GetEntityData().ScoreReward);
    }

  
    private void OnLevelUp(int level)
    {
        StartLevelUpAnimation();
    }

    private void StartLevelUpAnimation()
    {

    }

    public override EntityDataManager GetEntityData()
    {
        return PlayerData;
    }
}
