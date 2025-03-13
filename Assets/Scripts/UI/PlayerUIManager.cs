using System;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public event Action OnToggleUI;

    public bool IsUIOpen = false;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerInput.OnInventoryPressed += ToggleUI;
        _playerInput.OnSettingsPressed += ToggleUI;
    }

    private void OnDisable()
    {
        _playerInput.OnInventoryPressed -= ToggleUI;
        _playerInput.OnSettingsPressed -= ToggleUI;
    }

    private void ToggleUI()
    {
        IsUIOpen = ! IsUIOpen;
        OnToggleUI?.Invoke();
    }
}
