using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PlayerInteractionBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private CanvasGroup _canvasGroup;
    private PickUpManager _pickUpManager;

    private void Awake()
    {
        _pickUpManager = GetComponentInParent<PickUpManager>();
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide(); // Hide UI initially
    }

    private void OnEnable()
    {
        _pickUpManager.OnLootInRangeChanged += HandleLootRangeChange;

    }

    private void OnDisable()
    {
        _pickUpManager.OnLootInRangeChanged -= HandleLootRangeChange;

    }

    private void HandleLootRangeChange(bool isLootInRange, LootItem lootItem)
    {
        if (isLootInRange)
        {
            ShowLootItem(lootItem);
        }
        else
        {
            Hide();
        }
    }

    public void ShowLootItem(LootItem lootItem)
    {
        string interactionText = "PICK UP"; // change dynamically
        _text.text = $"{interactionText} {lootItem.ItemData.name}";

        _buttonText.text = "F";  // change dynamically
        Show();
    }

    private void Show()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }
}
