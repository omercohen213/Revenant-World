using UnityEngine;

public abstract class WeaponData : ItemData
{
    protected virtual bool IsAimable => false;
}
