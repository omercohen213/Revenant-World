using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour, IDamageReceiver
{
    public float MaxHealth { get; private set; }

    [ProgressBar("Health", "MaxHealth", EColor.Red)]
    [SerializeField] private float _currentHealth;
    public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }


    private Entity Owner;
    public UnityAction<float, GameObject> OnLostHealth { get; set; }

    public UnityAction<float> OnGainedHealth { get; set; }

    public UnityAction<EntityHealth, GameObject> OnHealthReachedZero { get; set; }

    public float GetRatio() => CurrentHealth / MaxHealth;

    public bool IsDead { get; private set; }

    protected virtual void Awake()
    {
        Owner = GetComponent<Entity>();
        if (Owner == null)
        {
            Debug.LogError("GameEntity component not found. Health cannot be initialized properly.");
        }
    }

    private void Start()
    {
        EntityRuntimeData owner = Owner.GetEntityData();
        MaxHealth = owner.BaseDataUntyped.MaxHealth;
        CurrentHealth = MaxHealth;
    }

  
    // Heal for heal amount
    public virtual void Heal(float healAmount)
    {
        float healthBefore = CurrentHealth;
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        // call OnHeal action
        float trueHealAmount = CurrentHealth - healthBefore;
        if (trueHealAmount > 0f)
        {
            OnGainedHealth?.Invoke(trueHealAmount);
        }
    }

    // Take damage from a damage source
    public virtual void TakeDamage(float damage, GameObject damageSource)
    {
        float healthBefore = CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        // call OnDamage action
        float trueDamageAmount = healthBefore - CurrentHealth;
        if (trueDamageAmount > 0f)
        {
            OnLostHealth?.Invoke(trueDamageAmount, damageSource);
        }

        CheckDeath(damageSource);
    }

    // Instantly kill the entity. Useful for testing
    public virtual void Kill()
    {
        CurrentHealth = 0f;

        OnLostHealth?.Invoke(MaxHealth, null);
        CheckDeath(Owner.gameObject);
    }

    // Check if the entity is dead and invoke OnKilled event
    protected virtual void CheckDeath(GameObject killer)
    {
        if (IsDead)
            return;

        // invoke OnKilled event
        if (CurrentHealth <= 0f)
        {
            IsDead = true;
            OnHealthReachedZero?.Invoke(this, killer);
        }
    }

    public void RecieveDamage(HitData hitData)
    {
        float healthBefore = CurrentHealth;
        CurrentHealth -= hitData.Damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        // call action
        float trueDamageAmount = healthBefore - CurrentHealth;
        if (trueDamageAmount > 0f)
        {
            OnLostHealth?.Invoke(trueDamageAmount, hitData.DamageSource);
        }

        CheckDeath(hitData.DamageSource);
    }
}


