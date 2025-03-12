using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Player))]
public class PickUpManager : MonoBehaviour
{
    private Player _player;
    private PlayerInput _playerInput;

    // List of loot items in range
    private List<LootItem> lootItemsInRange;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();
        lootItemsInRange = new List<LootItem>();
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
        foreach (LootItem loot in lootItemsInRange)
        {
            Debug.Log(loot);
        }
    }

    private void HandlePickUpPressed()
    {
        // Check if there are any loot items in range
        if (lootItemsInRange.Count > 0)
        {
            LootItem closestLoot = FindClosestItemInRange();
           
            // If a closest loot item is found, pick it up and remove it from the range
            if (closestLoot != null)
            {
                closestLoot.PickUp(_player);
                lootItemsInRange.Remove(closestLoot);
            }
        }
    }

    private LootItem FindClosestItemInRange()
    {
        // Find the closest loot item
        LootItem closestLoot = null;
        float closestDistance = float.MaxValue;

        foreach (LootItem loot in lootItemsInRange)
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
        LootItem loot = other.GetComponent<LootItem>();
        if (loot != null && !lootItemsInRange.Contains(loot))
        {
            lootItemsInRange.Add(loot);
        }
    }

    // Remove loot item when it leaves pickup range
    private void OnTriggerExit(Collider other)
    {
        LootItem loot = other.GetComponent<LootItem>();
        if (loot != null && lootItemsInRange.Contains(loot))
        {
            lootItemsInRange.Remove(loot);
        }
    }
}
