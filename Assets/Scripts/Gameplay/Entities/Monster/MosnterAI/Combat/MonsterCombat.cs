using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class MonsterCombat : IDisposable
{
    private MonsterAnimationEvents _animationEvents;
    private MonsterMovement _movement;
    private readonly TargetContext _targetContext;

    private IMonsterAbility _currentAbility;
    private MonsterAbilitySelector _abilitySelector;
    private CombatDecisionMaker _combatDecisionMaker;
    private MonsterCombatContext _combatContext;
    public bool IsAttacking => _currentAbility != null;
    public bool IsInCombat { get; private set; }

    public MonsterCombat(List<IMonsterAbility> abilities, MonsterAnimationEvents animationEvents, MonsterMovement movement, TargetContext targetContext)
    {
        _animationEvents = animationEvents;
        _movement = movement;
        _targetContext = targetContext;

        _animationEvents.AbilityAnimationEvent += HandleAbilityAnimationEvent;
        _abilitySelector = new MonsterAbilitySelector(abilities);
        _combatContext = new MonsterCombatContext(_targetContext, abilities);
        _combatDecisionMaker = new CombatDecisionMaker(_abilitySelector);
    }

    public void StartCombat()
    {
        Debug.Log("start combat");
        _combatContext.Reset();
        IsInCombat = true;
        _movement.Stop();
        ProcessCombatDecision();
    }


    public void Tick()
    {
        if (!IsInCombat)
            return;

        _combatContext.Tick();

        var ability = _currentAbility;

        if (ability != null)
        {
            ability.Tick();

            if (ability.IsFinished)
            {
                ability.End();
                _currentAbility = null;
            }
        }

        ProcessCombatDecision();
    }

    public void ProcessCombatDecision()
    {
        CombatDecision nextCombatDecision = _combatDecisionMaker.FindNextCombatDecision(_combatContext);
        switch (nextCombatDecision)
        {
            case CombatDecision.UseAbility:
                StartUseAbility();
                break;
            case CombatDecision.GetCloser:
                StartGetCloser();
                break;
            case CombatDecision.SearchTarget:
                StartSearchTarget();
                break;
        }
    }

    // move towards last seen target
    private void StartSearchTarget()
    {
        Debug.Log("searchTarget");
        _movement.MoveTo(_targetContext.LastSeenPosition);

    }

    private void StartGetCloser()
    {
        Debug.Log("getCloser"); 
        _movement.MoveTo(_targetContext.CurrentPosition);
    }

    private void StartUseAbility()
    {
        Debug.Log("useAbility");

        IMonsterAbility ability = _combatDecisionMaker.DecideAbility();

        if (ability != null)
        {
            TryStartAbility(ability);
        }
    }

    public bool TryStartAbility(IMonsterAbility ability)
    {
        if (_currentAbility != null)
            return false;
        _currentAbility = ability;
        ability.Begin();
        _movement.Stop();
        return true;
    }

    public bool CanAttack()
    {
        return true;
    }


    public void ExitCombat()
    {
        IsInCombat = false;
        if (_currentAbility != null)
        {
            _currentAbility.Cancel();
            _currentAbility = null;
        }
    }


    private void HandleAbilityAnimationEvent(AbilityAnimationEvent eventType)
    {
        if (_currentAbility is IAnimationDrivenAbility animationAbility)
        {
            animationAbility.OnAnimationEvent(eventType);
        }
    }

    public void Dispose()
    {
        _animationEvents.AbilityAnimationEvent -= HandleAbilityAnimationEvent;
    }

}
