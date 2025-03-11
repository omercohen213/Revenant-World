using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public float Weight = 1f;  // Affects inventory capacity
    public InventorySlot Slot;
    public bool ShowInInventory = true; // Some items are not shown in inventory
    public bool IsInfinite = false; // Infinite quantity in inventory
    public bool IsSignleInstance = false; // Single instance in inventory
}
