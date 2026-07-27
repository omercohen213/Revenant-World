using UnityEngine;

public abstract class MonsterAttackData : ScriptableObject
{
    public string AttackName;
    public float Cooldown;
    public float Range;

    public abstract IMonsterAttack Create(
        MonsterBrain brain,
        MonsterCombat combat,
        MonsterAnimationController animation,
        MonsterAttackPoints attackPoints);
}