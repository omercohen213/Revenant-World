using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventorySlot> InventorySlots;
    public List<ItemData> Items; // A list of unique items the invontory contains
    public int CurrentWeight = 0;
    public int Capacity;

    private readonly int _startingCapacity = 100;
    private readonly int _maxWeightPerSlot = 30;
    [SerializeField] private List<StartingItemEntry> _startingItems;
    [SerializeField] private ItemDataBase _itemDataBase; // A reference to the item database

    public Action<Weapon> OnWeaponAdded;

    private void Awake()
    {
        InventorySlots = new List<InventorySlot>();
        Items = new List<ItemData>();
    }

    private void Start()
    {
        Capacity = _startingCapacity;
        AddStartingItems();
        PrintInventory();
    }

    // Add starting items when the player joins the game
    private void AddStartingItems()
    {
        foreach (StartingItemEntry entry in _startingItems)
        {
            AddItemToInventory(entry.ItemData, entry.Quantity, entry.IsInfinite);
        }
    }

    // Check if item does not exceed the capacity
    public bool CanBeAdded(ItemData itemData, int quantity)
    {
        int weightToAdd = Mathf.RoundToInt(itemData.Weight * quantity);
        int totalWeight = CurrentWeight + weightToAdd;
        return totalWeight <= Capacity;
    }

    // Adds an item to the inventory
    public void AddItemToInventory(ItemData itemData, int quantity, bool isInfinite = false)
    {
        Debug.Log(CurrentWeight);
        if (isInfinite)
        {
            CreateInventorySlot(itemData, 999);
            return;
        }
        if (!CanBeAdded(itemData, quantity))
        {
            Debug.LogWarning("Cannot add item " + itemData + " : Exceeds total weight capacity.");
            return;
        }

        InventorySlot existingSlot = FindExistingSlot(itemData);
        int maxQuantityInSlot = GetMaxQuantityOfItemInSlot(itemData);

        // Add quantity to an existing slot
        if (existingSlot != null)
        {
            int availableSpace = maxQuantityInSlot - existingSlot.Quantity;
            int addQuantity = Mathf.Min(quantity, availableSpace);
            AddToInventorySlot(existingSlot, addQuantity);
            CurrentWeight += Mathf.RoundToInt(itemData.Weight * addQuantity);
            quantity -= addQuantity;
        }

        if (maxQuantityInSlot <= 0)
        {
            Debug.LogError($"Invalid max stack size for item {itemData.ItemName}. Preventing infinite loop.");
            return;
        }

        // Create new slots for any remaining quantity
        while (quantity > 0)
        {
            int slotQuantity = Mathf.Min(quantity, maxQuantityInSlot);
            CreateInventorySlot(itemData, slotQuantity);
            CurrentWeight += Mathf.RoundToInt(itemData.Weight * slotQuantity);
            quantity -= slotQuantity;
        }
    }

    // Add quantity of an existing slot
    private void AddToInventorySlot(InventorySlot slot, int quantity)
    {
        slot.Quantity += quantity;
        slot.Weight = Mathf.RoundToInt(slot.ItemData.Weight * slot.Quantity);
    }

    // Create a new slot
    private void CreateInventorySlot(ItemData item, int quantity)
    {
        if (item == null)
        {
            Debug.LogError("Tried to create an inventory slot with a null item!");
            return;
        }

        InventorySlots.Add(new InventorySlot(item, quantity));
    }

    // Return the max quantity of an item that can be inside a single slot 
    private int GetMaxQuantityOfItemInSlot(ItemData itemData)
    {
        if (itemData.IsInfinite) return int.MaxValue;
        if (itemData.Weight <= 0) return 1; // Ensure at least 1 item fits

        return Mathf.Max(1, Mathf.FloorToInt(_maxWeightPerSlot / itemData.Weight));
    }

    // Removes an inventory slot from the inventory
    public void RemoveInventorySlot(InventorySlot slot)
    {
        if (InventorySlots.Contains(slot))
        {
            CurrentWeight -= slot.Weight;
            InventorySlots.Remove(slot);
        }
        else
        {
            Debug.LogWarning("Attempted to remove a slot that does not exist in the inventory.");
        }
    }

    // Uses an item from the topmost slot containing the item
    public void ReduceItemQuantity(ItemData itemData, int quantity)
    {
        // If item is infinite, don't reduce quantity
        if (itemData.IsInfinite)
        {
            return;
        }

        InventorySlot slot = FindExistingSlot(itemData);
        if (slot == null)
        {
            Debug.LogWarning("Item not found in inventory.");
            return;
        }
        ReduceSlotQuantity(slot, quantity);
    }

    // Removes a specific quantity from a given inventory slot
    public void ReduceSlotQuantity(InventorySlot slot, int quantity)
    {
        if (InventorySlots.Contains(slot))
        {
            if (quantity >= slot.Quantity)
            {
                RemoveInventorySlot(slot);
            }
            else
            {
                slot.Quantity -= quantity;
                int weightRemoved = Mathf.RoundToInt(slot.ItemData.Weight * quantity);
                slot.Weight = Mathf.RoundToInt(slot.ItemData.Weight * slot.Quantity);
                CurrentWeight -= weightRemoved;
            }
        }
        else
        {
            Debug.LogWarning("Attempted to remove quantity from a slot that does not exist in the inventory.");
        }
    }

    // Find a slot with the same item; return null if does not exist
    private InventorySlot FindExistingSlot(ItemData itemData)
    {
        foreach (InventorySlot slot in InventorySlots)
        {
            if (slot.ItemData.ItemName == itemData.ItemName)
            {
                return slot;
            }
        }
        return null;
    }

    // Get the total quantity of the item across all inventory slots
    public int GetTotalQuantityOfItem(ItemData itemData)
    {
        int total = 0;
        foreach (InventorySlot slot in InventorySlots)
        {
            if (slot.ItemData.ItemName == itemData.ItemName)
            {
                total += slot.Quantity;
            }
        }
        return total;
    }

    public void PrintInventory()
    {
        Debug.Log("Total Capacity: " + Capacity + " Current Weight: " + CurrentWeight);

        string inventoryText = "Current Inventory: ";


        foreach (InventorySlot slot in InventorySlots)
        {
            inventoryText += $"{slot.ItemData.ItemName}, Quantity: {slot.Quantity}, Weight: {slot.Weight} | ";
        }

        Debug.Log(inventoryText);
    }

}

public class InventorySlot
{
    public ItemData ItemData { get; private set; }
    public int Quantity { get; set; }
    public int Weight { get; set; }

    public InventorySlot(ItemData itemData, int quantity)
    {
        ItemData = itemData;
        Quantity = quantity;
        Weight = Mathf.RoundToInt(itemData.Weight * quantity);
    }
}

[System.Serializable]
public class StartingItemEntry
{
    public ItemData ItemData;
    public int Quantity;
    public bool IsInfinite;

    public StartingItemEntry(ItemData itemData, int quantity, bool isInfinite)
    {
        ItemData = itemData;
        Quantity = quantity;
        IsInfinite = isInfinite;
    }
}
