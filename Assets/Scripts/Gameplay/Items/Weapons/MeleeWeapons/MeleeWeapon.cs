using UnityEngine;

public abstract class MeleeWeapon : Weapon, IMeleeWeapon
{
    public void BasicAttack()
    {
        throw new System.NotImplementedException();
    }

    public void HeavyAttack()
    {
        throw new System.NotImplementedException();
    }
}
