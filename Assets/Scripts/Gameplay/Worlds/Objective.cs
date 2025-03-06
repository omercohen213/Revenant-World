using System;
using UnityEngine;

public abstract class Objective : ScriptableObject
{
    public string ObjectiveDescription;
    public event Action OnObjectiveCompleted;
    private bool isCompleted = false; // Track if objective is already completed
    public abstract bool IsCompleted();
    public abstract void Initialize();
    protected abstract void GiveCompletionRewards();

    protected void CompleteObjective()
    {
        if (isCompleted) return; // Prevent multiple completions

        isCompleted = true; // Mark as completed
        GiveCompletionRewards();
        OnObjectiveCompleted?.Invoke();
    }

    // Reset completion status for reusability
    public void ResetObjective()
    {
        isCompleted = false; 
    }
}
