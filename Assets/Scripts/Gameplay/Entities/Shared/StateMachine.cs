using UnityEngine;

public class StateMachine
{
    private IState _currentState;

    public virtual void ChangeState(IState newState)
    {
        _currentState?.Exit();

        _currentState = newState;

        _currentState.Enter();
    }

    public virtual void Tick()
    {
        _currentState?.Tick();
    }
}
