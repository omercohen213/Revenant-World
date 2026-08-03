using UnityEngine;

public class MonsterCombatState : State
{
    private MonsterCombat _combat;

    public MonsterCombatState(
        MonsterRuntimeData runtimeData,
        MonsterCombat combat)
        : base(runtimeData)
    {
        _combat = combat;
    }

    public override void Enter()
    {
        _combat.StartCombat();
    }

    public override void Tick()
    {
    }

    public override void Exit()
    {
        _combat.ExitCombat();
        Debug.Log("exit combat");
    }
}