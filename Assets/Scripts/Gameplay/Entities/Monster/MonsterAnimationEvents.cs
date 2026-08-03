using System;
using UnityEngine;

public class MonsterAnimationEvents : MonoBehaviour
{
    public event Action<AbilityAnimationEvent> AbilityAnimationEvent;


    public void OnAnimationEvent(AbilityAnimationEvent type)
    {
        AbilityAnimationEvent?.Invoke(type);
    }
}

public enum AbilityAnimationEvent
{
    Release,
    HitboxStart,
    HitboxEnd,
    Finished,
}
