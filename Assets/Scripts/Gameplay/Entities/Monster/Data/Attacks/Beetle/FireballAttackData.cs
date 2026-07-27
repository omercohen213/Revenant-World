using UnityEngine;

[CreateAssetMenu(menuName = "Monster/Attacks/Fireball")]
public class FireballAttackData : MonsterAttackData
{
    public float Damage;
    public Projectile ProjectilePrefab;
    public ProjectileData ProjectileData;

    public override IMonsterAttack Create(
       MonsterBrain brain,
       MonsterCombat combat,
       MonsterAnimationController animation, 
       MonsterAttackPoints attackPoints)
    {
        return new FireballAttack(brain, combat, this, animation, attackPoints);
    }
}