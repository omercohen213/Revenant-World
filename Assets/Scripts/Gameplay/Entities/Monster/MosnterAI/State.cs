using UnityEngine;

public class State : IState
{
    protected readonly MonsterBrain _brain;
    protected readonly MonsterRuntimeData _runtimeData;
    protected readonly MonsterMovement _monsterMovement;
    protected readonly MonsterAnimationController _animController;

    protected State(
        MonsterBrain brain,
        MonsterRuntimeData runtimeData,
        MonsterMovement movement,
        MonsterAnimationController animController
        )
    {
        _brain = brain;
        _runtimeData = runtimeData;
        _monsterMovement = movement;
        _animController = animController;
    }

    public virtual void Enter() { }

    public virtual void Tick() { }

    public virtual void Exit() { }

}
