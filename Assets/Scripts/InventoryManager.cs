using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Weapon StartingWeapon;
    public List<Item> Items;
    public int totalAmmo = 1000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Weapon GetActiveWeapon()
    {
        return StartingWeapon;
    }

    public int GetTotalAmmo (Weapon weapon)
    {
        return totalAmmo;
    }

    public void UseItem(Item item, int Amount)
    {

    }

    public void UseAmmo(int amount)
    {
        totalAmmo -= amount;
    }
}
