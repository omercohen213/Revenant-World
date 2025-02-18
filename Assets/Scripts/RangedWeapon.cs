using System;
using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    public RangedWeaponData RangedWeaponData;

    public event Action OnShoot;
    public event Action OnReload;

    public int CurrentAmmo;

    protected void InvokeShoot() => OnShoot?.Invoke();
    protected void InvokeReload() => OnReload?.Invoke();

    public virtual void StartShooting()
    {

    }

    public virtual void ContinueShooting()
    {

    }

    public virtual void StopShooting()
    {

    }


    public virtual bool TryShoot()
    {
        return false;
    }

    protected virtual void HandleShoot()
    {

    }
    public virtual void Reload(int ammoToReload)
    {
    
    }
    public virtual void StartReloadAnimation()
    {
        //GetComponent<Animator>().SetTrigger("Reload");
    }

    public virtual void StopReloadAnimation()
    {
        //GetComponent<Animator>().enabled = false;
    }

    /*    public void SubscribeToShoot(Action action) => OnShoot += action;
        public void SubscribeToReload(Action action) => OnReload += action;*/
}
