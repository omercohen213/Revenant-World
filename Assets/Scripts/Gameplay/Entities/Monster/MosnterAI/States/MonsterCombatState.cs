using UnityEngine;

public class MonsterCombatState : State
{
    private MonsterCombat _monsterCombat;
    private CombatDecisionMaker _decisionMaker;

    public MonsterCombatState(
        MonsterBrain brain,
        MonsterRuntimeData runtimeData,
        MonsterMovement movement,
        MonsterAnimationController animation,
        CombatDecisionMaker decisionMaker)
        : base(brain, runtimeData, movement, animation)
    {
        _decisionMaker = decisionMaker;
    }

    public override void Enter()
    {
        _monsterMovement.Stop();
    }

    public override void Tick()
    {
        _decisionMaker.Tick();
    }

    public override void Exit()
    {
        Debug.Log("exit combat");
    }
}