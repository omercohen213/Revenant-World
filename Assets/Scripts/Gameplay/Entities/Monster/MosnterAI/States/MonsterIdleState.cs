using Unity.VisualScripting;
using UnityEngine;

public class MonsterIdleState : State
{
    public MonsterIdleState(
        MonsterBrain brain,
        MonsterRuntimeData runtimeData,
        MonsterMovement movement,
        MonsterAnimationController animation)
        : base(brain, runtimeData, movement, animation)
    {
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