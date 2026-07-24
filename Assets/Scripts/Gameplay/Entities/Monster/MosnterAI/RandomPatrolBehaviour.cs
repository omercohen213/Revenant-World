using UnityEngine;

public class RandomPatrolBehaviour : IPatrolBehaviour
{
    private readonly MonsterMovement _movement;
    private readonly PatrolArea _area;

    private float _stoppingDelay;
    private float _timer;
    private bool _waiting;

    public RandomPatrolBehaviour(
        MonsterMovement movement,
        PatrolArea area,
        float stoppingDelay)
    {
        _movement = movement;
        _area = area;
        _stoppingDelay = stoppingDelay;
    }

    public void Enter()
    {
        _waiting = false;
        PickNewDestination();
    }

    public void Tick()
    {
        if (_waiting)
        {
            _timer += Time.deltaTime;

            if (_timer >= _stoppingDelay)
            {
                _waiting = false;
                PickNewDestination();
            }

            return;
        }

        if (_movement.HasReachedDestination())
        {
            _movement.Stop();

            _waiting = true;
            _timer = 0f;
        }
    }

    public void Exit()
    {
    }

    private void PickNewDestination()
    {
        _movement.Resume();
        _movement.MoveTo(_area.GetRandomPoint());
    }
}
