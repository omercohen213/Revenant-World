using UnityEngine;

[CreateAssetMenu(menuName = "Monster/Abilities/Bite")]
public class BiteAbilityData : MonsterAbilityData
{
    public float Damage;
    public float AttackMovementDistance;
    public override Color DebugColor => Color.yellow;

    public override IMonsterAbility Create(MonsterAbilitiyContext context)
    {
        return new Bite(context, this);
    }
}