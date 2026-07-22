using UnityEngine;

// High level class for Monster AI behaviour
[RequireComponent(typeof(MonsterRuntimeData))]
[RequireComponent(typeof(EntityHealth))]
public class MonsterBrain : MonoBehaviour
{
    [Header("References")]
    private MonsterRuntimeData _data;
    private EntityHealth _health;
    [SerializeField] private LayerMask _playerLayer;

    public MonsterTargetSensor Sensor { get; private set; }

    public MonsterIdleState IdleState { get; private set; }
    public MonsterChaseState ChaseState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }
    public MonsterReturnState ReturnState { get; private set; }

    private StateMachine _stateMachine;

    private bool _hasFled = false;


    private void Awake()
    {
        _data = GetComponent<MonsterRuntimeData>();
        _health = GetComponent<EntityHealth>();
        _stateMachine = new StateMachine();

        Sensor = new MonsterTargetSensor(_playerLayer);
        //Movement = new MonsterMovement();

        IdleState = new MonsterIdleState();
        ChaseState = new MonsterChaseState();
        AttackState = new MonsterAttackState();
        ReturnState = new MonsterReturnState();
    }

    private void Start()
    {
        _stateMachine.ChangeState(IdleState);
    }

    private void Update()
    {
        _stateMachine.Tick();
        if (!_hasFled && _health.CurrentHealth < 50f)
        {
            _stateMachine.ChangeState(ChaseState);
            _hasFled = true;
        }

    }
}