using UnityEngine;

public abstract class MonsterAbilityData : ScriptableObject
{
    public string AttackName;
    public virtual Color DebugColor => Color.white;
    public string AnimationParameter;
    public float AnimationSpeed = 1f;
    
    //public float WindupDuration; // attack preperation time
    //public float RecoveryDuration; // recovery time after attack ended
    public float BaseCooldown; 
    public float Range;

    public abstract IMonsterAbility Create(MonsterAbilitiyContext context);
}