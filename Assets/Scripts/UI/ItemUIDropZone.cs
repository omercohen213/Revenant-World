using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUIDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Get the dropped object from the pointer data.
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem != null)
        {
            Debug.Log($"{droppedItem.name} dropped on {gameObject.name}");
            // Set the dropped item's parent to this drop zone.
            droppedItem.transform.SetParent(transform, false);
        }
        else
        {
            Debug.LogWarning("No item was found to drop.");
        }
    }
}
