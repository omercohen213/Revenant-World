using UnityEngine;

//[CreateAssetMenu(fileName = "NewRangedWeaponData", menuName = "Inventory/RangedWeapon Data")]
public abstract class RangedWeaponData : WeaponData
{
    [Header("Reload")]
    public float AmmoReloadDelay = 2f;
    public float ReloadTime = 3f;

    [Header("Ammo")]
    public bool HasAmmo = false;
    public int ClipSize = 30;
}
