using UnityEngine;

public abstract class WeaponData : ItemData
{
    public Projectile ProjectilePrefab;
    public ProjectileData ProjectileData;
    public float ProjectileDamage;

    protected virtual bool IsAimable => false;
}
