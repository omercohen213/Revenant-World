using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public GameObject ItemPrefab;
    public string ItemName;
    public Sprite Icon;

    [Header("Default Position on Player")]
    public Vector3 DefaultPosition;
    public Vector3 DefaultLocalEulerAngles; // rotation in degrees

    [Header("Inventory")]
    public ItemType ItemType;
    public float Weight = 1f;  // Affects inventory capacity
    public InventorySlot Slot;
    public bool IsEquippable => ItemType == ItemType.Weapon;
    public bool ShowInInventory = true; // Some items are not shown in inventory
    public bool IsInfinite = false; // Infinite quantity in inventory
    public bool IsSignleInstance = false; // Single instance in inventory
}

public enum ItemType
{
    Weapon,
    Ammo,
    Consumable,
}