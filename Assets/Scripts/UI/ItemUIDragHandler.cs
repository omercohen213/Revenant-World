using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;
    private Canvas _canvas;
    private Vector2 _originalAnchoredPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        
        // Record the starting anchored position so we can reset if necessary.
        _originalAnchoredPosition = _rectTransform.anchoredPosition;
    }

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("No Canvas found in parent hierarchy for " + gameObject.name);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Record original parent to return if not dropped in a valid target.
        _originalParent = transform.parent;
        // Bring item to the top-level of the Canvas.
        transform.SetParent(_canvas.transform, true);
        _canvasGroup.alpha = 0.6f;  // Make the item slightly transparent.
        _canvasGroup.blocksRaycasts = false;  // Allow raycasts to pass through during dragging.
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_canvas == null)
            return;
        // Move the rectTransform based on cursor movement.
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        // Check if dropped onto a valid drop zone.
        if (!TryDropItem(eventData))
        {
            // Return to original parent and reset position if drop is invalid.
            transform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }
    }

    /// <summary>
    /// Attempts to drop the item onto a valid drop zone.
    /// A valid drop zone is identified by a DropZone component in its parent hierarchy.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    /// <returns>True if dropped onto a valid zone; otherwise, false.</returns>
    private bool TryDropItem(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            // Try to get a DropZone component from the target or its parents.
            ItemUIDropZone dropZone = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<ItemUIDropZone>();
            if (dropZone != null)
            {
                transform.SetParent(dropZone.transform, false);
                return true;
            }
        }
        return false;
    }
}
