
using UnityEngine;

public abstract class RangedWeaponData : WeaponData
{
    [Header("Reload")]
    public bool HasAmmo = true;
    public AmmoData RequiredAmmo;
    public float ReloadTime = 3f;
    public int ClipSize = 30;
    protected override bool IsAimable => true;
}
