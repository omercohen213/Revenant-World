using System;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement: IDisposable
{
    public float CurrentSpeed => _agent.velocity.magnitude;
    private readonly NavMeshAgent _agent;

    public MonsterMovement(NavMeshAgent agent, float movementSpeed)
    {
        _agent = agent;
        _agent.speed = movementSpeed;
    }

    public void Tick()
    {
    }

    public void Stop()
    {
        _agent.isStopped = true;
    }

    public void Resume()
    {
        _agent.isStopped = false;
    }


    public void MoveTo(Vector3 position)
    {
        _agent.isStopped = false;
        _agent.SetDestination(position);
    }

    public bool HasReachedDestination()
    {
        if (_agent.pathPending)
        {
            return false;
        }

        if (_agent.remainingDistance > _agent.stoppingDistance)
        {
            return false;
        }

        return !_agent.hasPath || _agent.velocity.sqrMagnitude < 0.01f;
    }

    public void Dispose()
    {
    }
}