using UnityEngine;


// Context to access monster data during runtime
public class MonsterRuntimeData
{
    //public MonsterAIController Controller { get; }
    public MonsterDataManager Data { get; }
    public Transform Transform { get; }
    public Transform Target { get; set; }
    public Vector3 SpawnPosition { get; }
    public float StateTimer { get; set; }
    public float AttackTimer { get; set; }
    public float DecisionTimer { get; set; }
    public float MoveSpeed { get; set; }

    /*public MonsterRuntimeContext(MonsterAIController controller, MonsterDataManager data)
    {
        Controller = controller;
        Data = data;
        Transform = controller.transform;
        SpawnPosition = controller.transform.position;
    }*/
}