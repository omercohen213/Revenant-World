using System.Collections.Generic;
using UnityEngine;

// Select an attack between all available ones
public class MonsterAbilitySelector
{
    private readonly IReadOnlyList<IMonsterAbility> _abilities;

    public MonsterAbilitySelector(List<IMonsterAbility> abilities)
    {
        _abilities = abilities;
    }


    public IMonsterAbility ChooseAbility()
    {
        List<IMonsterAbility> usable = new();


        foreach (var ability in _abilities)
        {
            if (ability.CanUse())
            {
                usable.Add(ability);
            }
        }

        if (usable.Count == 0)
            return null;


        return usable[Random.Range(0, usable.Count)];
    }
}