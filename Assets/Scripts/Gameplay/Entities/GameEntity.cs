using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
public abstract class GameEntity : MonoBehaviour
{
    protected Health _health;

    protected virtual void Awake()
    {
        _health = GetComponent<Health>();

    }

    protected virtual void Start()
    {

    }

    protected virtual void OnEnable()
    {
        _health.OnKilled += HandleDeath;
    }

    protected virtual void OnDisable()
    {
        _health.OnKilled -= HandleDeath;
    }

    protected virtual void HandleDeath(Health health, GameObject killer)
    {
        // If the killer is a player, they get rewards
        if (killer.TryGetComponent<Player>(out var killerPlayer))
        {
            killerPlayer.GetRewardForKill(this);
        }
    }
       

    public virtual GameEntityDataManager GetEntityData()
    {
        return null;
    }
}
