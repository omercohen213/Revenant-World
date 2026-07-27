using GLTF.Schema;
using Micosmo.SensorToolkit;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

// High level class for Monster AI behaviour
[RequireComponent(typeof(MonsterRuntimeData))]
[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(PatrolArea))]
public class MonsterBrain : MonoBehaviour
{
    [Header("References")]
    private MonsterRuntimeData _data;
    private MonsterAnimationController _animation;
    private MonsterMovement _monsterMovement;
    private MonsterCombat _monsterCombat;
    private CombatDecisionMaker _combatDecisionMaker;
    private AttackSelector _attackSelector;
    private List<IMonsterAttack> _attacks;
    private MonsterAttackPoints _attackPoints;
    private StateMachine _stateMachine;
    private PatrolArea _patrolArea;
    private LOSSensor _sensor;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _stateEvaluationRate = 0.2f;

    private float _evaluationTimer;

    public Player Target { get; private set; }
    public MonsterIdleState IdleState { get; private set; }
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterCombatState CombatState { get; private set; }

    public bool HasTarget => Target != null;
    public float DistanceToTarget => Target == null ? Mathf.Infinity : Vector3.Distance(transform.position, Target.transform.position);

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _data = GetComponent<MonsterRuntimeData>();
        _animation = GetComponentInChildren<MonsterAnimationController>();
        _monsterMovement = GetComponent<MonsterMovement>();
        _monsterCombat = GetComponent<MonsterCombat>();
        _patrolArea = GetComponent<PatrolArea>();
        _sensor = GetComponent<LOSSensor>();

        _attacks = new List<IMonsterAttack>();
        _attackPoints = GetComponent<MonsterAttackPoints>();

        foreach (MonsterAttackData attackData in _data.BaseData.Attacks)
        {
            _attacks.Add(attackData.Create(this,_monsterCombat,_animation, _attackPoints));
        }

        _stateMachine = new StateMachine();
        _attackSelector = new AttackSelector(_attacks);
        _combatDecisionMaker = new CombatDecisionMaker(_attackSelector, _monsterCombat);
        RandomPatrolBehaviour patrolBehaviour = new(_monsterMovement, _patrolArea, _data.BaseData.PatrolStoppingDelay);

        IdleState = new MonsterIdleState(this, _data, _monsterMovement, _animation);
        PatrolState = new MonsterPatrolState(this, _data, _monsterMovement, _animation, patrolBehaviour);
        CombatState = new MonsterCombatState(this, _data, _monsterMovement, _animation, _combatDecisionMaker);
    }

    private void Update()
    {
        TryFindTarget();

        _stateMachine.Tick();

        _evaluationTimer += Time.deltaTime;

        if (_evaluationTimer >= _stateEvaluationRate)
        {
            _evaluationTimer = 0f;
            EvaluateStateTransitions();
        }
    }

    public void EvaluateStateTransitions()
    {
        if (HasTarget)
        {
            if (_stateMachine.CurrentState != CombatState)
            {
                _stateMachine.ChangeState(CombatState);
            }

            return;
        }

        if (_stateMachine.CurrentState != PatrolState)
        {
            _stateMachine.ChangeState(PatrolState);
        }
    }

    private void TryFindTarget()
    {
        if (HasTarget)
            return;
        
        Target = _sensor.GetNearestComponent<Player>();
    }
}
