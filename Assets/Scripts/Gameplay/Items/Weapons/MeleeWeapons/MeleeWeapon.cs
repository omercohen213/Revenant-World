using UnityEngine;

public abstract class MeleeWeapon : Weapon, IMeleeWeapon
{
    public void HandleBasicAttack()
    {
        throw new System.NotImplementedException();
    }

    public void HandleHeavyAttack()
    {
        throw new System.NotImplementedException();
    }
}
