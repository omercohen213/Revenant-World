using UnityEngine;

public interface IRangedWeapon : IWeapon
{
    // Basic shooting methods
    void HandleFireDown();
    void HandleFireHeld();
    void HandleFireReleased();
    void HandleShoot();
    void Reload(int ammoToReload);

    // Aiming behavior
    void StartAiming();
    void StopAiming();
}