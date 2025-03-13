using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Player))]
public class PickUpManager : MonoBehaviour
{
    public event Action<bool,LootItem> OnLootInRangeChanged;

    private Player _player;
    private PlayerInput _playerInput;

    // List of loot items in range
    private List<LootItem> _lootItemsInRange;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();
        _lootItemsInRange = new List<LootItem>();
    }

    private void OnEnable()
    {
        _playerInput.OnPickUpPressed += HandlePickUpPressed;
    }

    private void OnDisable()
    {
        _playerInput.OnPickUpPressed -= HandlePickUpPressed;

    }

    private void Update()
    {
        foreach (LootItem loot in _lootItemsInRange)
        {
            Debug.Log(loot);
        }
    }

    private void HandlePickUpPressed()
    {
        // Check if there are any loot items in range
        if (_lootItemsInRange.Count > 0)
        {
            LootItem closestLoot = FindClosestItemInRange();
           
            // If a closest loot item is found, pick it up and remove it from the range
            if (closestLoot != null)
            {
                closestLoot.PickUp(_player);
                _lootItemsInRange.Remove(closestLoot);
            }
            if (_lootItemsInRange.Count == 0)
            {
                OnLootInRangeChanged?.Invoke(false,null); // Fire event when no more loot is in range
            }
        }
    }

    private LootItem FindClosestItemInRange()
    {
        // Find the closest loot item
        LootItem closestLoot = null;
        float closestDistance = float.MaxValue;

        foreach (LootItem loot in _lootItemsInRange)
        {
            float distance = Vector3.Distance(_player.transform.position, loot.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLoot = loot;
            }
        }
        return closestLoot;
    }

    // Called when a loot item enters the player's pickup range
    private void OnTriggerEnter(Collider other)
    {
        LootItem lootItem = other.GetComponent<LootItem>();
        if (lootItem != null && !_lootItemsInRange.Contains(lootItem))
        {
            _lootItemsInRange.Add(lootItem);
        }
        if (_lootItemsInRange.Count == 1)
        {
            OnLootInRangeChanged?.Invoke(true,lootItem); // Fire event when first loot enters range
        }
    }

    // Remove loot item when it leaves pickup range
    private void OnTriggerExit(Collider other)
    {
        LootItem loot = other.GetComponent<LootItem>();
        if (loot != null && _lootItemsInRange.Contains(loot))
        {
            _lootItemsInRange.Remove(loot);
        }
        if (_lootItemsInRange.Count == 0)
        {
            OnLootInRangeChanged?.Invoke(false, null); // Fire event when first loot enters range
        }
    }
}
