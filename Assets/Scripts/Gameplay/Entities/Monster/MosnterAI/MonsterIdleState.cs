using UnityEngine;

public class MonsterIdleState : IState
{
    public void Enter()
    {
        Debug.Log("enterIdle");
    }

    public void Exit()
    {
        Debug.Log("ExitIdle");
    }

    public void Tick()
    {
        Debug.Log("Idle");
    }
}