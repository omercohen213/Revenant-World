using UnityEngine;

[CreateAssetMenu(menuName = "Monster/Abilities/Fireball")]
public class FireballAbilityData : MonsterAbilityData
{
    public float Damage;
    public Projectile ProjectilePrefab;
    public ProjectileData ProjectileData;
    public override Color DebugColor => Color.red;

    public override IMonsterAbility Create(MonsterAbilitiyContext context)
    {
        return new Fireball(context, this);
    }
}