using UnityEngine;
using UnityEngine.Events;

public abstract class Health : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;

    public UnityAction<float, GameObject> OnDamaged;
    public UnityAction<float> OnHealed;
    public UnityAction OnDie;

    public bool Invincible { get; set; }
    public bool CanPickup() => CurrentHealth < MaxHealth;

    public float GetRatio() => CurrentHealth / MaxHealth;

    private bool _isDead;

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;
    }

    protected virtual void Update()
    {
        //TakeDamage(1f, GetComponent<Entity>().gameObject);
    }

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

        CheckDeath();
    }

    public virtual void Kill()
    {
        CurrentHealth = 0f;

        // call OnDamage action
        OnDamaged?.Invoke(MaxHealth, null);

        CheckDeath();
    }

    protected virtual void CheckDeath()
    {
        if (_isDead)
            return;

        // call OnDie action
        if (CurrentHealth <= 0f)
        {
            _isDead = true;
            OnDie?.Invoke();
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


