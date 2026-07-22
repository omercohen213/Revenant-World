using UnityEngine;

public class MonsterChaseState : IState
{
    public void Enter()
    {
        Debug.Log("enterChase");
    }

    public void Exit()
    {
        Debug.Log("exitChase");
    }

    public void Tick()
    {
        Debug.Log("TickChase");
    }
}