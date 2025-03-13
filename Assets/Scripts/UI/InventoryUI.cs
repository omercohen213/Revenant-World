using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _itemUIPrefab;
    [SerializeField] private Transform _inventoryItems;
    [SerializeField] private Transform _lootItems;
    [SerializeField] private float fadeDuration = 0.3f;

    private bool _isInventoryOpen = false;
    private CanvasGroup _inventoryCanvasGroup;
    private PlayerInput _playerInput;

    private InventoryManager _inventoryManager;

    private ObjectPool<GameObject> _itemUIPool;

    private void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _inventoryCanvasGroup = GetComponent<CanvasGroup>();
        _inventoryManager = GetComponentInParent<InventoryManager>();
        _itemUIPool = ObjectPoolingManager.Instance.GetOrCreatePool(_itemUIPrefab, defaultCapacity: 10, maxSize: 100);
    }

    private void OnEnable()
    {
        _playerInput.OnInventoryPressed += HandleInventoryPressed;
    }

    private void OnDisable()
    {
        _playerInput.OnInventoryPressed -= HandleInventoryPressed;
    }

    // Toggle the inventory UI open/closed and refresh its content if opening.
    private void HandleInventoryPressed()
    {
        StopAllCoroutines();
        int targetAlpha = _isInventoryOpen ? 0 : 1;
        StartCoroutine(FadeInventory(_inventoryCanvasGroup, targetAlpha, fadeDuration));

        if (!_isInventoryOpen)
        {
            RefreshInventoryUI();
            RefreshLootUI();
        }
    }

    private IEnumerator FadeInventory(CanvasGroup canvasGroup, int targetAlpha, float fadeDuration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        _isInventoryOpen = !_isInventoryOpen;
        canvasGroup.interactable = targetAlpha > 0;
        canvasGroup.blocksRaycasts = targetAlpha > 0;
    }

    // Refresh the inventory UI by releasing old UI elements back to the pool
    // and then getting new ones based on the current inventory data.
    public void RefreshInventoryUI()
    {
        // Release all current UI items in the inventory panel back to the pool.
        foreach (Transform child in _inventoryItems)
        {
            _itemUIPool.Release(child.gameObject);
        }

        // Populate the inventory UI from the InventoryManager's slots.
        foreach (InventorySlot slot in _inventoryManager.InventorySlots)
        {
            GameObject itemUI = _itemUIPool.Get();
            itemUI.transform.SetParent(_inventoryItems, false);
            if (itemUI.TryGetComponent<ItemUI>(out var uiComponent))
            {
                uiComponent.Setup(slot);
            }
        }
    }

    // Refreshes the loot UI. Modify this method to work with your loot system.
    // For now, it releases any current loot UI elements.
    public void RefreshLootUI()
    {
        foreach (Transform child in _lootItems)
        {
            _itemUIPool.Release(child.gameObject);
        }
        // If you have a loot system, you can iterate over its items here similar to RefreshInventoryUI.

       
    }


}