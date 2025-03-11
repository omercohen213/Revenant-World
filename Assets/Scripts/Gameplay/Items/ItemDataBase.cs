using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Inventory/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    public List<ItemData> Items;

    public ItemData GetItemByName(string name)
    {
        foreach (var item in Items)
        {
            if (item.ItemName == name)
            {
                return item;
            }
        }

        Debug.LogWarning($"Item with name '{name}' not found in the database.");
        return null; // Return null if item is not found
    }
}
