using System;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterRuntimeData))]
public class MonsterMovement : MonoBehaviour
{
    private MonsterAnimationController _animationController;
    private NavMeshAgent _agent;
    private MonsterRuntimeData _data;

    private void Awake()
    {
        _animationController = GetComponentInChildren<MonsterAnimationController>();
        _agent = GetComponent<NavMeshAgent>();
        _data = GetComponent<MonsterRuntimeData>();
    }

    private void Start()
    {
        _agent.speed = _data.BaseData.MovementSpeed;
    }

    private void Update()
    {
        float currentSpeed = _agent.velocity.magnitude;
        _animationController.SetMovementSpeed(currentSpeed);
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
            return false ;
        }

        return !_agent.hasPath || _agent.velocity.sqrMagnitude < 0.01f;
    }
}