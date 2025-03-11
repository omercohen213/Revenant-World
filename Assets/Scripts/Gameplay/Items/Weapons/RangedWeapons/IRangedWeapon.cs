using UnityEngine;

public interface IRangedWeapon : IWeapon
{
    // Basic shooting methods
    void StartShooting();
    void ContinueShooting();
    void StopShooting();
    void HandleShoot();
    void Reload(int ammoToReload);

    // Aiming behavior
    void StartAiming();
    void StopAiming();
}