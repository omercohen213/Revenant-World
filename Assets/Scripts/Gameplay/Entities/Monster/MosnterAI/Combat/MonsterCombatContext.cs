using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Context information during monster combat
public class MonsterCombatContext
{
    public readonly List<IMonsterAbility> _abilities;
    public readonly TargetContext _context;

    public List<IMonsterAbility> AbilitiesInRange;

    public MonsterCombatContext(TargetContext context, List<IMonsterAbility> abilities)
    {
        _context = context;
        _abilities = abilities;
        AbilitiesInRange = new();
    }

    public void Tick()
    {
        AbilitiesInRange = CheckAbilitiesInRange();
    }

    private List<IMonsterAbility> CheckAbilitiesInRange()
    {
        AbilitiesInRange.Clear();
        foreach (IMonsterAbility ability in _abilities)
        {
            if (!AbilitiesInRange.Contains(ability) && ability.IsInRange())
            {
                    AbilitiesInRange.Add(ability);
            }
        }
        return AbilitiesInRange;
    }

    public void Reset()
    {
        AbilitiesInRange.Clear();
    }
}
