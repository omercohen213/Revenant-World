using UnityEngine;

public interface IRangedWeapon : IWeapon
{
    void HandleShoot();
    void Reload(int ammoToReload);
}