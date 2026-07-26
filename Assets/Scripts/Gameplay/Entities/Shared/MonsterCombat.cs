using System.Collections.Generic;
using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    private readonly List<IMonsterAttack> _attacks = new();
    private IMonsterAttack _currentAttack;

    public bool IsAttacking => _currentAttack != null;

    private void Update()
    {
        if (_currentAttack == null)
            return;

        _currentAttack.Tick();

        if (_currentAttack.Finished)
        {
            _currentAttack.End();
            _currentAttack = null;
        }
    }

    public void AddAttack(IMonsterAttack attack)
    {
        _attacks.Add(attack);
    }

    public bool TryStartAttack(IMonsterAttack attack)
    {
        if (_currentAttack != null)
            return false;

        _currentAttack = attack;
        attack.Begin();
        return true;
    }

    public bool CanAttack()
    {
        return true;
    }
}
