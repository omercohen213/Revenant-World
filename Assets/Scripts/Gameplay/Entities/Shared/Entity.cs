using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[RequireComponent(typeof(EntityHealth))]
public abstract class Entity : MonoBehaviour
{
    protected EntityHealth _health;

    public UnityAction<Entity, GameObject> OnKilled { get; set; } // Monster killed, Killer

    protected virtual void Awake()
    {
        _health = GetComponent<EntityHealth>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnEnable()
    {
        _health.OnHealthReachedZero += HandleDeath;
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void OnDisable()
    {
        _health.OnHealthReachedZero -= HandleDeath;
    }

    protected virtual void HandleDeath(EntityHealth health, GameObject killerObject)
    {
        // If the killer is a player, they get rewards
        if (killerObject.TryGetComponent<Player>(out var killerPlayer))
        {
            killerPlayer.GetRewardForKill(this);
            OnKilled?.Invoke(this, killerPlayer.gameObject);
        }
    }

    public virtual EntityRuntimeData GetEntityData()
    {
        return null;
    }

    protected virtual void OnDestroy()
    {
        
    }
}
