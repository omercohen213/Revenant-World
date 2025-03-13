using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LootItem : MonoBehaviour, ILootItem
{
    public ItemData ItemData;
    public int Quantity;

    [SerializeField] private Player _owner;

    public void PickUp(Player player)
    {
        _owner = player;

        if (player.TryGetComponent<InventoryManager>(out var inventory))
        {
            if (inventory.CanBeAdded(ItemData, Quantity))
            {
                Debug.Log("pick up");
                inventory.AddItemToInventory(ItemData, Quantity);
                Destroy(gameObject); // Remove from the scene after pickup
            }
            else
            {
                Debug.LogWarning($"Cannot pick up {ItemData.ItemName}, not enough inventory space.");
            }
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void Drop()
    {
        throw new System.NotImplementedException();
    }
}
