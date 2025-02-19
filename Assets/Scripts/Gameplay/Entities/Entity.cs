using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Entity : MonoBehaviour
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
        gameObject.SetActive(false);
        
        // If the killer is a player, they get rewards
        if (killer.TryGetComponent<Player>(out var killerPlayer))
        {
            killerPlayer.GetRewardForKill(this);
        }
    }
       

    public virtual EntityDataManager GetEntityData()
    {
        return null;
    }
}
