using Micosmo.SensorToolkit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterStateMachine : IDisposable
{
    private readonly StateMachine _stateMachine;
    private readonly MonsterStateEvaluator _stateEvaluator;

    private readonly MonsterPatrolState _patrolState;
    private readonly MonsterCombatState _combatState;

    private readonly TargetContext _targetContext;


    public MonsterStateMachine(
        TargetContext targetContext,
        MonsterPatrolState patrolState,
        MonsterCombatState combatState,
        float stateEvaluationRate)
    {
        _targetContext = targetContext;
        _patrolState = patrolState;
        _combatState = combatState;

        _stateMachine = new StateMachine();
        _stateEvaluator = new MonsterStateEvaluator(targetContext, stateEvaluationRate);
    }

    public void Tick()
    {
        _stateMachine.Tick();

        if (!_stateEvaluator.ShouldEvaluate(Time.deltaTime))
            return;

        MonsterStateType nextState = _stateEvaluator.Evaluate();

        switch (nextState)
        {
            case MonsterStateType.Combat:
                if (_stateMachine.CurrentState != _combatState)
                {
                    _stateMachine.ChangeState(_combatState);
                }
                break;

            case MonsterStateType.Patrol:
                if (_stateMachine.CurrentState != _patrolState)
                {
                    _stateMachine.ChangeState(_patrolState);
                }
                break;
        }
    }

    public void Dispose()
    {
    }
}
