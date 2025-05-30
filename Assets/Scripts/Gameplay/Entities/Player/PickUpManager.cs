using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Player))]
public class PickUpManager : MonoBehaviour
{
    public event Action<LootItem> OnLootInRangeChanged;

    [SerializeField] Transform _itemPickUpPoint; // A transform to determine the distance to the closest item

    private Player _player;
    private PlayerInput _playerInput;
    private List<LootItem> _lootItemsInRange;
    private LootItem _lastClosestItem;

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

    private void Start()
    {
        _lastClosestItem = null;
    }

    private void Update()
    {
        LootItem closestItem = FindClosestItemInRange();
        
        // Fire an event when the closest item is changed
        if (closestItem != _lastClosestItem)
        {
            _lastClosestItem = closestItem;
            OnLootInRangeChanged?.Invoke(closestItem);
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
        }
    }

    // Find the closest item of the items in range
    private LootItem FindClosestItemInRange()
    {
        // Find the closest loot item
        LootItem closestLoot = null;
        float closestDistance = float.MaxValue;

        foreach (LootItem loot in _lootItemsInRange)
        {
            float distance = Vector3.Distance(_itemPickUpPoint.position, loot.transform.position);

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
    }

    // Remove loot item when it leaves pickup range
    private void OnTriggerExit(Collider other)
    {
        LootItem loot = other.GetComponent<LootItem>();
        if (loot != null && _lootItemsInRange.Contains(loot))
        {
            _lootItemsInRange.Remove(loot);
        }
    }
}
