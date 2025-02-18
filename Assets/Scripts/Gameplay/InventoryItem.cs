using UnityEngine;

public abstract class InventoryItem: ScriptableObject
{
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public float Weight;  // Affects inventory capacity
    public int MaxStack;  // Max number of this item in one slot

    public virtual void Use() { } // Overridden by specific item types
}
