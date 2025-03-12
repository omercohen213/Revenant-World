using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Animator")]
    private Animator _animator;
    [Tooltip("Crosshair")]
    [SerializeField] private Crosshair _crosshair;

    [Header("Misc")]
    [Tooltip("Delay before switching weapon a second time, to avoid recieving multiple inputs from mouse wheel")]
    public float WeaponSwitchDelay = 1f;

    public bool IsAiming { get; private set; }

    private Player _player;
    private InventoryManager _inventoryManager;
    private IWeapon _activeWeapon;

    private Coroutine _reloadCoroutine;

    void Start()
    {
        if (!DebugUtil.SafeGetComponent(gameObject, out _inventoryManager)) return;
        if (!DebugUtil.SafeGetComponent(gameObject, out _player)) return;
        SetActiveWeapon(_player.ActiveWeapon);
    }

    public void SetActiveWeapon(IWeapon weapon)
    {
        _activeWeapon = weapon;
    }

    private void UpdateActiveWeapon()
    {
        _activeWeapon = _player.ActiveWeapon;
    } 
}