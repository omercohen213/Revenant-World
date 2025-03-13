
using UnityEngine;

public abstract class RangedWeaponData : WeaponData
{
    [Header("Shooting")]
    public float DelayBetweenShots = 0.1f;

    [Header("Aiming")]
    public float AimZoomRatio = 1.5f;
    public Vector3 AimOffset;
    public float AimingAnimationSpeed = 10f;

    [Header("Reload")]
    public bool HasAmmo = true;
    public AmmoData RequiredAmmo;
    public float ReloadTime = 3f;
    public int ClipSize = 30;
    protected override bool IsAimable => true;
}
