using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "KillObjective", menuName = "Game/Objective/Kill")]
public class KillObjective : Objective
{
    public int RequiredKills;
    public int CurrentKills;

    public override void Initialize()
    {
        CurrentKills = 0;
        ResetObjective();
    }

    public void RegisterKill()
    {
        CurrentKills++;

        if (IsCompleted())
        {
            CompleteObjective();
        }
    }

    public override bool IsCompleted()
    {
        return CurrentKills >= RequiredKills;
    }

    // Give rewards to all players who assisted in completing this objective
    protected override void GiveCompletionRewards()
    {
        Debug.Log("Rewards granted for completing the objective!");
    }
}
