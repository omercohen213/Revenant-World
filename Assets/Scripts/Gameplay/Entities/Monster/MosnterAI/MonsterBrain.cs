using GLTF.Schema;
using UnityEngine;

// High level class for Monster AI behaviour
[RequireComponent(typeof(MonsterRuntimeData))]
[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(PatrolArea))]
public class MonsterBrain : MonoBehaviour
{
    [Header("References")]
    private MonsterRuntimeData _data;
    private MonsterAnimationController _animation;
    private MonsterMovement _movment;
    private PatrolArea _patrolArea;
    [SerializeField] private LayerMask _playerLayer;

    public MonsterTargetSensor Sensor { get; private set; }

    public MonsterIdleState IdleState { get; private set; }
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterChaseState ChaseState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }
    public StateMachine StateMachine { get; private set; }

    private void Awake()
    {
        _data = GetComponent<MonsterRuntimeData>();
        _animation = GetComponentInChildren<MonsterAnimationController>();
        _movment = GetComponent<MonsterMovement>(); 
        _patrolArea = GetComponent<PatrolArea>();
        StateMachine = new StateMachine();

        Sensor = new MonsterTargetSensor(_playerLayer);

        IdleState = new MonsterIdleState(this,_data,_movment,_animation);

        RandomPatrolBehaviour patrolBehaviour = new (_movment,_patrolArea, _data.BaseData.PatrolStoppingDelay); // Might want to move it from here
        PatrolState = new MonsterPatrolState(this,_data,_movment,_animation, patrolBehaviour);
        
        ChaseState = new MonsterChaseState();
        AttackState = new MonsterAttackState();

    }

    private void Start()
    {
        StateMachine.ChangeState(PatrolState);
    }

    private void Update()
    {
        StateMachine.Tick();
    }

}