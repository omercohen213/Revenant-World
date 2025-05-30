using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

[RequireComponent(typeof(PlayerDataManager))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(PlayerInput))]
public class Player : GameEntity
{
    public PlayerDataManager PlayerData;
    public World CurrentWorld;

    public event Action<GameEntity, int, int> OnKillRewarded;  // Killed entity, XP, KP
    private ObjectPool<Player> _playerPool;

    protected override void Awake()
    {
        base.Awake();
        PlayerData = GetComponent<PlayerDataManager>();
        _playerPool = ObjectPoolingManager.Instance.GetOrCreatePool(this);
        CurrentWorld = GetComponentInParent<World>();
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

    // Add the rewards to the player data
    public void GetRewardForKill(GameEntity killedEntity)
    {
        int xp = killedEntity.GetEntityData().XpReward;
        int kp = killedEntity.GetEntityData().KpReward;

        PlayerData.AddXp(xp);
        PlayerData.AddKp(kp);

        OnKillRewarded?.Invoke(killedEntity, xp, kp);
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
