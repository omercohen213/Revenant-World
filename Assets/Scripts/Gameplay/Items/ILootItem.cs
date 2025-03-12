using UnityEngine;

public interface ILootItem
{
    void PickUp(Player player);
    void Drop();
}
