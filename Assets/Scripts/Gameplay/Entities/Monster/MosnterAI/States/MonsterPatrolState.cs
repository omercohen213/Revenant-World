using GLTF.Schema;
using UnityEngine;

public class MonsterPatrolState : State
{
    private readonly IPatrolBehaviour _patrolBehaviour;

    public MonsterPatrolState(
        MonsterBrain brain,
        MonsterRuntimeData runtimeData,
        MonsterMovement movement,
        MonsterAnimationController animation,
        IPatrolBehaviour patrolBehaviour)
        : base(brain, runtimeData, movement, animation)
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

        // Detect player here...
    }

    public override void Exit()
    {
        _patrolBehaviour.Exit();
    }
}
