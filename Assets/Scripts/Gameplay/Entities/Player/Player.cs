using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

[RequireComponent(typeof(PlayerDataManager))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InventoryManager))]
public class Player : GameEntity
{
    public PlayerDataManager PlayerData;
    public Weapon ActiveWeapon;
    public Weapon StartingWeapon;

    private ObjectPool<Player> _playerPool;


    protected override void Awake()
    {
        base.Awake();
        PlayerData = GetComponent<PlayerDataManager>();
        ActiveWeapon = StartingWeapon;
        _playerPool = ObjectPoolingManager.Instance.GetOrCreatePool(this);
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

    public void GetRewardForKill(GameEntity entity)
    {
        PlayerData.AddXP(entity.GetEntityData().XpReward);
        PlayerData.AddScore(entity.GetEntityData().ScoreReward);
    }

    protected override void HandleDeath(Health health, GameObject killer)
    {
        _playerPool.Release(this);
        base.HandleDeath(health, killer);
    }

    private void OnLevelUp(int level)
    {
        StartLevelUpAnimation();
    }

    private void StartLevelUpAnimation()
    {

    }

    public override GameEntityDataManager GetEntityData()
    {
        return PlayerData;
    }
}
