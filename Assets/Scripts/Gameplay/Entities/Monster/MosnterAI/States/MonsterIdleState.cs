using Unity.VisualScripting;
using UnityEngine;

public class MonsterIdleState : State
{
    private MonsterMovement _monsterMovement;

    public MonsterIdleState(
        MonsterRuntimeData runtimeData,
        MonsterMovement movement)
        : base(runtimeData)
    {
        _monsterMovement = movement;
    }
  
    public override void Enter()
    {
        Debug.Log("Idle");
        _monsterMovement.Stop();
    }

    public override void Tick()
    {
    }
}