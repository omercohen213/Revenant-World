using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo", menuName = "Items/Ammo")]
public class AmmoData : ItemData
{
    public AmmoType Type;
    public List<WeaponType> CompatibleWeapons; // List of weapon types this ammo works with
}

public enum AmmoType
{
    LightAmmo,
    HeavyAmmo,
    EnergyAmmo,
    NormalArrows,
    ExplosiveArrows
}