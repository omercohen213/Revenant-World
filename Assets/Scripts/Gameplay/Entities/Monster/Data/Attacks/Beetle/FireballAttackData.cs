using UnityEngine;

[CreateAssetMenu(menuName = "Monster/Attacks/Fireball")]
public class FireballAttackData : MonsterAttackData
{
    public float Damage;
    public GameObject ProjectilePrefab;

    public override IMonsterAttack Create(
       MonsterBrain brain,
       MonsterCombat combat,
       MonsterAnimationController animation)
    {
        return new FireballAttack(brain, combat, this, animation);
    }
}