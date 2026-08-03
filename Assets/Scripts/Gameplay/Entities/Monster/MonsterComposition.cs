using Micosmo.SensorToolkit;
using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using static UnityEngine.UI.GridLayoutGroup;

// Resoponsible for each concrete system in a monster
public class MonsterComposition: IDisposable
{
    private readonly MonsterReferences _references;


    public MonsterStateMachine MonsterStateMachine { get; private set; }
    public MonsterMovement Movement { get; private set; }
    public MonsterTargeting Targeting { get; private set; }
    public MonsterCombat Combat { get; private set; }
    public MonsterAbilitySelector AbilitySelector { get; private set; }
    public CombatDecisionMaker CombatDecisionMaker { get; private set; }
    public MonsterIdleState IdleState { get; private set; }
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterCombatState CombatState { get; private set; }

    private List<IMonsterAbility> _abilities;


    public MonsterComposition(MonsterReferences references)
    {
        _references = references;
    }


    public void Build()
    {
        Movement = new MonsterMovement(_references.Agent, _references.Data.BaseData.MovementSpeed);
        Targeting = new MonsterTargeting(_references.Sensor, _references.Data);

        BuildAbilities();
        Combat = new MonsterCombat(_abilities, _references.AnimationEvents, Movement, Targeting.TargetContext);
        
        BuildStates();
        MonsterStateMachine = new MonsterStateMachine(Targeting.TargetContext, PatrolState, CombatState, 0.2f);
    }

    private void BuildStates()
    {
        RandomPatrolBehaviour patrolBehaviour = new(Movement, _references.PatrolArea, _references.Data.BaseData.PatrolStoppingDelay);
        PatrolState = new MonsterPatrolState(_references.Data, patrolBehaviour);
        IdleState = new MonsterIdleState(_references.Data, Movement);
        CombatState = new MonsterCombatState(_references.Data, Combat);
    }

    private void BuildAbilities()
    {
        _abilities = new();

        foreach (var abilityData in _references.Data.BaseData.Abilities)
        {
            MonsterAbilitiyContext context = new(_references.Owner, Targeting.TargetContext, _references.AnimationController, _references.AttackPoints);
            _abilities.Add(abilityData.Create(context));
        }
    }
    public void Tick(float deltaTime)
    {
        Targeting.Tick();
        MonsterStateMachine.Tick();
        Combat.Tick();
    }

    // For object pooling
    public void Reset()
    {
        //Brain.Reset();
        //Combat.Reset();
        //Targeting.Reset();
    }


    public void Dispose()
    {
        MonsterStateMachine.Dispose();
        Combat.Dispose();
        Targeting.Dispose();
        Movement.Dispose();
    }

}
