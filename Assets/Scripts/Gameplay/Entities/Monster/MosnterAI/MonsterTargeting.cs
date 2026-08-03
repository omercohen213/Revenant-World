using Micosmo.SensorToolkit;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterTargeting : IDisposable
{
    private readonly MonsterRuntimeData _data;
    private readonly LOSSensor _sensor;

    private TargetContext _targetContext;
    private float _lostTargetTime;
    private readonly float _targetMemory = 10f;

    public TargetContext Context => _targetContext;

    public TargetContext TargetContext { get => _targetContext; }

    public MonsterTargeting(LOSSensor sensor, MonsterRuntimeData data)
    {
        _sensor = sensor;
        _data = data;
        _targetContext = new TargetContext();
    }

    public void Tick()
    {
        TryFindTarget();
    }

    private void TryFindTarget()
    {
        Player newTarget = _sensor.GetNearestComponent<Player>();
        // Target is visable
        if (newTarget != null)
        {
            _targetContext.SetTarget(newTarget);
            _lostTargetTime = 0f;

            UpdateCurrentTargetContext();
            UpdateLastTargetContext();
        }
        else
        {
            _targetContext.LoseSight();

            _lostTargetTime += Time.deltaTime;

            if (_lostTargetTime >= _targetMemory)
            {
                _targetContext.ForgetTarget();
            }
        }
    }


    private void UpdateCurrentTargetContext()
    {
        if (!_targetContext.HasTarget)
            return;

        _targetContext.UpdateCurrentPosition(_targetContext.Target.transform.position);
    }

    private void UpdateLastTargetContext()
    {
        _targetContext.UpdateLastSeenPosition();
    }


    public void Dispose()
    {
    }
}