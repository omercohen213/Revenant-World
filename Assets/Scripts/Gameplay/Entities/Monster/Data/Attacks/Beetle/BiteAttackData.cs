using UnityEngine;

[CreateAssetMenu(menuName = "Monster/Attacks/Bite")]
public class BiteAttackData : MonsterAttackData
{
    public float Damage;

    public override IMonsterAttack Create(
       MonsterBrain brain,
       MonsterCombat combat,
       MonsterAnimationController animation,
       MonsterAttackPoints attackPoints)
    {
        return new BiteAttack(brain, combat, this, animation, attackPoints);
    }
}