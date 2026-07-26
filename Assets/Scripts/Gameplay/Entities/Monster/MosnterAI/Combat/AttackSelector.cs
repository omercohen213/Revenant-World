using System.Collections.Generic;
using UnityEngine;

public class AttackSelector
{
    private readonly IReadOnlyList<IMonsterAttack> _attacks;


    public AttackSelector(List<IMonsterAttack> attacks)
    {
        _attacks = attacks;
    }


    public IMonsterAttack ChooseAttack()
    {
        List<IMonsterAttack> usable = new();


        foreach (var attack in _attacks)
        {
            if (attack.CanUse())
            {
                usable.Add(attack);
            }
        }


        if (usable.Count == 0)
            return null;


        return usable[Random.Range(0, usable.Count)];
    }
}