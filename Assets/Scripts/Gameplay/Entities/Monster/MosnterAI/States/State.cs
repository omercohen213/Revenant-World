using UnityEngine;

public class State : IState
{
    protected readonly MonsterRuntimeData _runtimeData;

    protected State(MonsterRuntimeData runtimeData)
    {
        _runtimeData = runtimeData;
    }

    public virtual void Enter() { }

    public virtual void Tick() { }

    public virtual void Exit() { }
}

public enum MonsterStateType
{
    Idle,
    Patrol,
    Combat, // seperate to attack, chase, retreat
    Stunned,
    Dead
      
}
