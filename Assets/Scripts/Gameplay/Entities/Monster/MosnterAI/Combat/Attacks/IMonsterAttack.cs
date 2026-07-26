using UnityEngine;

public interface IMonsterAttack
{
    bool CanUse();

    void Begin();

    void Tick();

    void End();

    bool Finished { get; }
}
