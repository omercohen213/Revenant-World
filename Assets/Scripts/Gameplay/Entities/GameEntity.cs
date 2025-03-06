using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
public abstract class GameEntity : MonoBehaviour
{
    protected Health _health;

    public UnityAction<GameEntity, GameObject> OnKilled; // Monster killed, Killer

    protected virtual void Awake()
    {
        _health = GetComponent<Health>();

    }

    protected virtual void Start()
    {

    }

    protected virtual void OnEnable()
    {
        _health.OnHealthReachedZero += HandleDeath;
    }

    protected virtual void OnDisable()
    {
        _health.OnHealthReachedZero -= HandleDeath;
    }

    protected virtual void HandleDeath(Health health, GameObject killerObject)
    {
        // If the killer is a player, they get rewards
        if (killerObject.TryGetComponent<Player>(out var killerPlayer))
        {
            killerPlayer.GetRewardForKill(this);
            OnKilled.Invoke(this, killerPlayer.gameObject);
        }
    }


    public virtual GameEntityDataManager GetEntityData()
    {
        return null;
    }
}
