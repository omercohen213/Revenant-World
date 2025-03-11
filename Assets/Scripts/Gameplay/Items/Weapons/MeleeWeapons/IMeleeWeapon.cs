using UnityEngine;

public interface IMeleeWeapon : IWeapon
{
    void HandleBasicAttack();
    void HandleHeavyAttack();
}
