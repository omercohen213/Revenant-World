using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> Items;
    public int totalAmmo = 1000;

    public int GetTotalAmmo (Weapon weapon)
    {
        return totalAmmo;
    }

    public void UseItem(InventoryItem item, int Amount)
    {

    }

    public void UseAmmo(int amount)
    {
        totalAmmo -= amount;
    }
}
