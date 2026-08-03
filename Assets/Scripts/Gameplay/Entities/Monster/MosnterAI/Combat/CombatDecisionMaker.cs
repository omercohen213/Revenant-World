using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Decide which next action to make during combat
public class CombatDecisionMaker
{
    private readonly MonsterAbilitySelector _abilitySelector;

    public CombatDecisionMaker(MonsterAbilitySelector selector)
    {
        _abilitySelector = selector;
    }

    // Rule based combat decision
    public CombatDecision FindNextCombatDecision(MonsterCombatContext context)
    {

        if (context._context.HasTarget)
        {
            if (context._context.CanSeeTarget) {
                if (context.AbilitiesInRange.Count > 0)
                {
                    return CombatDecision.UseAbility;
                }
                // might want to use gap closer ability to get closer here
                return CombatDecision.GetCloser;
            }
            // in combat but cant see target
             return CombatDecision.SearchTarget;
        }
        else
        {
            Debug.LogWarning("In combat with no target");
            return CombatDecision.Stay; 
        }
    }

    public IMonsterAbility DecideAbility()
    {
        return _abilitySelector.ChooseAbility();
    }
}

public enum CombatDecision
{
    UseAbility,
    TacticalReposition,
    SearchTarget,
    GetCloser,
    Stay,
    Run,
    Hide
}
