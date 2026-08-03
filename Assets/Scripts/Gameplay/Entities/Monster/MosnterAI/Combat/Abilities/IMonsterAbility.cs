using UnityEngine;

public interface IMonsterAbility
{
    void Begin();
    void Tick();
    void Cancel();
    void End();

    bool CanUse();
    bool IsInRange();
    bool IsFinished { get; }
}
