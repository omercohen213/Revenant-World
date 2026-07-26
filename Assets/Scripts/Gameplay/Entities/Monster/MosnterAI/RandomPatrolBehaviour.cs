using System;
using UnityEngine;

public class RandomPatrolBehaviour : IPatrolBehaviour
{
    private readonly MonsterMovement _monsterMovement;
    private readonly PatrolArea _area;

    private bool _breakPatrolBehaviour; // flag for stopping directly the patrol
    private float _stoppingDelay;
    private float _timer;
    private bool _waiting;

    public RandomPatrolBehaviour(
        MonsterMovement movement,
        PatrolArea area,
        float stoppingDelay)
    {
        _monsterMovement = movement;
        _area = area;
        _stoppingDelay = stoppingDelay;
    }

    public void Enter()
    {
        _breakPatrolBehaviour = false;
        _waiting = false;
        PickNewDestination();
    }

    public void Tick()
    {
        if (_breakPatrolBehaviour)
        {
            _monsterMovement.Stop();
            return;
        }

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

        if (_monsterMovement.HasReachedDestination())
        {
            _monsterMovement.Stop();

            _waiting = true;
            _timer = 0f;
        }
    }

    public void Exit()
    {
        _breakPatrolBehaviour = true;
    }

    private void PickNewDestination()
    {
        _monsterMovement.Resume();
        _monsterMovement.MoveTo(_area.GetRandomPoint());
    }
}
