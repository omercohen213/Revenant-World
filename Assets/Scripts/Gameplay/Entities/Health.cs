using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float MaxHealth { get; private set; }
    [ProgressBar("Health", "MaxHealth", EColor.Red)]
    [SerializeField] private float CurrentHealth;

    private GameEntity Owner;
    public UnityAction<float, GameObject> OnDamaged;
    public UnityAction<float> OnHealed;
    public UnityAction<Health, GameObject> OnKilled;

    public bool Invincible { get; set; }

    public float GetRatio() => CurrentHealth / MaxHealth;

    public bool IsDead { get; private set; }

    protected virtual void Awake()
    {
        Owner = GetComponent<GameEntity>();
        if (Owner == null)
        {
            Debug.LogError("GameEntity component not found. Health cannot be initialized properly.");
        }
    }

    private void Start()
    {
        MaxHealth = Owner.GetEntityData().baseData.MaxHealth;
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
            OnHealed?.Invoke(trueHealAmount);
        }
    }

    // Take damage from a damage source
    public virtual void TakeDamage(float damage, GameObject damageSource)
    {
        if (Invincible)
            return;

        float healthBefore = CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        // call OnDamage action
        float trueDamageAmount = healthBefore - CurrentHealth;
        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }

        CheckDeath(damageSource);
    }

    // Instantly kill the entity. Useful for testing
    public virtual void Kill()
    {
        CurrentHealth = 0f;

        // call OnDamage action
        OnDamaged?.Invoke(MaxHealth, null);
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
            OnKilled?.Invoke(this, killer);
        }
    }

    /*private IEnumerator RegenerateCoroutine()
{
   while (true)
   {
       if (isInCombat)
       {
           yield return new WaitForSeconds(inCombatDelay);
           isInCombat = false; // set isInCombat back to false after inCombatDelay
       }
       else
       {
           bool shouldRegenerateHp = hp < maxHp;

           if (shouldRegenerateHp)
           {
               // Regenerate HP
               hp += hpRegen;
               if (hp > maxHp)
               {
                   hp = maxHp;
               }
               hud.onHpChange();
           }

           // Wait for the next regeneration tick
           yield return new WaitForSeconds(regenDelay);
       }
   }*/
}


