using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _iconImage;       // The icon representing the item.
    [SerializeField] private TextMeshProUGUI _itemNameText;     // Display the item name.
    [SerializeField] private TextMeshProUGUI _quantityText;     // Display the quantity.

    // Initializes the UI with data from the inventory slot.
    public void Setup(InventorySlot slot)
    {
        if (slot == null || slot.ItemData == null)
        {
            Debug.LogWarning("Invalid inventory slot or missing item data.");
            return;
        }

        // Update the icon image if available.
        if (_iconImage != null && slot.ItemData.Icon != null)
        {
            _iconImage.sprite = slot.ItemData.Icon;
        }
        else if (_iconImage != null)
        {
            // Optionally, clear the icon or set a default icon.
            _iconImage.sprite = null;
        }

        // Update the item name text.
        if (_itemNameText != null)
        {
            _itemNameText.text = slot.ItemData.ItemName;
        }

        // Update the quantity text.
        if (_quantityText != null)
        {
            _quantityText.text = slot.Quantity.ToString();
        }
    }
}
