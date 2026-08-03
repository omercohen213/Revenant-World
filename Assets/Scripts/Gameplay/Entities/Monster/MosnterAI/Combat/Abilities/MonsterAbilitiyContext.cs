using UnityEngine;

public class MonsterAbilitiyContext 
{
    public Monster Owner { get; }
    public TargetContext TargetContext { get; }
    public MonsterAnimationController AnimController { get; }
    public MonsterAttackPoints AttackPoints { get; }

    public MonsterAbilitiyContext(
        Monster owner,
        TargetContext targetContext,
        MonsterAnimationController animation,
        MonsterAttackPoints attackPoints)
    {
        Owner = owner;
        TargetContext = targetContext;
        AnimController = animation;
        AttackPoints = attackPoints;
    }
}
