using UnityEngine;

public class State : IState
{
    protected readonly MonsterBrain _brain;
    protected readonly EntityRuntimeData _runtimeData;
    protected readonly MonsterMovement _movement;

    protected State(
        MonsterBrain brain,
        EntityRuntimeData runtimeData,
        MonsterMovement movement
        )
    {
        _brain = brain;
        _runtimeData = runtimeData;
        _movement = movement;
    }

    public virtual void Enter() { }

    public virtual void Tick() { }

    public virtual void Exit() { }

}
