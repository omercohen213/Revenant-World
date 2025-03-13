using System;
using UnityEngine;
using UnityEngine.UI;

public class UICursor : MonoBehaviour
{
    [SerializeField] private RectTransform _cursor;
    private PlayerUIManager _playerUIManager;

    private void Awake()
    {
        _playerUIManager = GetComponentInParent<PlayerUIManager>();
    }

    private void OnEnable()
    {
        _playerUIManager.OnToggleUI += HandleToggleUI;
    }

    private void OnDisable()
    {
        _playerUIManager.OnToggleUI -= HandleToggleUI;
    }

    void Start()
    {
        Cursor.visible = false;
        LockCursor();
    }

    void Update()
    {
        if (_playerUIManager.IsUIOpen)
        {
            _cursor.position = Input.mousePosition;
        }
    }

    private void HandleToggleUI()
    {
        if (_playerUIManager.IsUIOpen)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
