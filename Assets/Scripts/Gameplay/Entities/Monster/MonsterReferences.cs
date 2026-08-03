using Micosmo.SensorToolkit;
using UnityEngine.AI;

public class MonsterReferences
{
    public Monster Owner { get; }
    public MonsterRuntimeData Data { get; }
    public NavMeshAgent Agent { get; }
    public LOSSensor Sensor { get; }
    public MonsterAttackPoints AttackPoints { get; }
    public MonsterAnimationController AnimationController { get; }
    public MonsterAnimationEvents AnimationEvents { get; }
    public PatrolArea PatrolArea { get; }

    public MonsterReferences(
        Monster owner,
        MonsterRuntimeData data,
        NavMeshAgent agent,
        LOSSensor sensor,
        MonsterAttackPoints attackPoints,
        MonsterAnimationController animationController,
        MonsterAnimationEvents animationEvents,
        PatrolArea patrolArea)
    {
        Owner = owner;
        Data = data;
        Agent = agent;
        Sensor = sensor;
        AttackPoints = attackPoints;
        AnimationController = animationController;
        AnimationEvents = animationEvents;
        PatrolArea = patrolArea;
    }
}
