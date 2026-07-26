using UnityEngine;

public class CombatDecisionMaker
{
    private readonly AttackSelector _selector;
    private readonly MonsterCombat _combat;


    public CombatDecisionMaker(
        AttackSelector selector,
        MonsterCombat combat)
    {
        _selector = selector;
        _combat = combat;
    }


    public void Tick()
    {
        if (_combat.IsAttacking)
            return;


        IMonsterAttack attack = _selector.ChooseAttack();


        if (attack != null)
        {
            _combat.TryStartAttack(attack);
        }
    }
}
