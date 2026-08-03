using UnityEngine;

public class MonsterStateEvaluator
{
    private readonly TargetContext _targetContext;

    private readonly float _evaluationInterval;
    private float _evaluationTimer;

    public MonsterStateEvaluator(
        TargetContext targetContext,
        float evaluationInterval)
    {
        _targetContext = targetContext;
        _evaluationInterval = evaluationInterval;
    }

    public bool ShouldEvaluate(float deltaTime)
    {
        _evaluationTimer += deltaTime;

        if (_evaluationTimer < _evaluationInterval)
            return false;

        _evaluationTimer = 0f;
        return true;
    }

    public MonsterStateType Evaluate()
    {
        if (_targetContext.HasTarget)
            return MonsterStateType.Combat;

        return MonsterStateType.Patrol;
    }
}