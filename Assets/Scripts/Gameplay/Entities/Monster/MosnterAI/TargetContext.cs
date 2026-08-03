using UnityEngine;

public class TargetContext
{
    public Player Target { get; private set; }
    public bool HasTarget => Target != null;
    public bool CanSeeTarget { get; private set; }

    public Vector3 CurrentPosition { get; private set; }
    public Vector3 LastSeenPosition { get; private set; }

    public void SetTarget(Player target)
    {
        Target = target;
        CanSeeTarget = true;
    }

    public void LoseSight()
    {
        CanSeeTarget = false;
    }

    public void ForgetTarget()
    {
        Target = null;
        CanSeeTarget = false;
    }

    public void UpdateCurrentPosition(Vector3 position)
    {
        CurrentPosition = position;
    }

    public void UpdateLastSeenPosition()
    {
        LastSeenPosition = CurrentPosition;
    }


    public void UpdateVisiblePosition(Vector3 position)
    {
        CurrentPosition = position;
        LastSeenPosition = position;
    }

}
