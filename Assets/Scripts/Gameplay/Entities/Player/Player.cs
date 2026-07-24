using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

[RequireComponent(typeof(PlayerRuntimeData))]
[RequireComponent(typeof(EntityHealth))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(PlayerInput))]
public class Player : Entity
{
    public PlayerRuntimeData PlayerData;
    public World CurrentWorld;

    public event Action<Entity, int, int> OnKillRewarded;  // Killed entity, XP, KP
    private ObjectPool<Player> _playerPool;

    protected override void Awake()
    {
        base.Awake();
        PlayerData = GetComponent<PlayerRuntimeData>();
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
    public void GetRewardForKill(Entity killedEntity)
    {
        int xp = killedEntity.GetEntityData().XpReward;
        int kp = killedEntity.GetEntityData().KpReward;

        PlayerData.AddXp(xp);
        PlayerData.AddKp(kp);

        OnKillRewarded?.Invoke(killedEntity, xp, kp);
    }

    protected override void HandleDeath(EntityHealth health, GameObject killer)
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

    public override EntityRuntimeData GetEntityData()
    {
        return PlayerData;
    }
}
