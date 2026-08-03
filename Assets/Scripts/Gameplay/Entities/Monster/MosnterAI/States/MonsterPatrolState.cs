using GLTF.Schema;
using UnityEngine;

public class MonsterPatrolState : State
{
    private readonly IPatrolBehaviour _patrolBehaviour;

    public MonsterPatrolState(
        MonsterRuntimeData runtimeData,
        IPatrolBehaviour patrolBehaviour)
        : base(runtimeData)
    {
        _patrolBehaviour = patrolBehaviour;
    }

    public override void Enter()
    {
        _patrolBehaviour.Enter();
    }

    public override void Tick()
    {
        _patrolBehaviour.Tick();
    }

    public override void Exit()
    {
        _patrolBehaviour.Exit();
    }
}
