using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.DebugUI;


public class WeaponManager : MonoBehaviour
{
    public Weapon ActiveWeapon;

    [SerializeField] private WeaponData _defaultWeaponData;
    [SerializeField] private Transform _weaponSocket; // Parent of the weapon to instantiate into

    private List<InventorySlot> _equippedWeaponSlots;
    private int _currentWeaponIndex = 0;

    private ObjectPool<GameObject> _WeaponsPool;
    private PlayerInput _playerInput;
    private InventoryManager _inventoryManager;
    private readonly float _weaponSwitchDelay = 1f;

    public Action<Weapon> OnWeaponChanged;
    public WeaponData GetStartingWeaponData() { return _defaultWeaponData; }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _inventoryManager = GetComponent<InventoryManager>();
    }

    void Start()
    {
        _equippedWeaponSlots = _inventoryManager.InventorySlots.FindAll(slot => slot.ItemData.IsEquippable);
        CreateDefaultWeapon();
    }

    private void OnEnable()
    {
        _playerInput.OnWeaponSwitched += SwitchWeapon;
    }

    private void OnDisable()
    {
        _playerInput.OnWeaponSwitched -= SwitchWeapon;
    }

    // Create the first weapon at the start of the game
    private void CreateDefaultWeapon()
    {


/*        _currentWeaponIndex = 0;
        _weapons.Add(defualtWeapon);
        ActivateWeapon(_currentWeaponIndex);
        OnWeaponChanged?.Invoke(_weapons[_currentWeaponIndex]);

*/
        // Make sure the default weapon is in the inventory
        if (!_inventoryManager.Items.Contains(_defaultWeaponData))
        {
            _inventoryManager.AddItemToInventory(_defaultWeaponData, 1);
        }
        GameObject defaultWeaponPrefab = _defaultWeaponData.ItemPrefab;
        Weapon defualtWeapon = defaultWeaponPrefab.GetComponent<Weapon>();
        GameObject weaponGameObject = Instantiate(defaultWeaponPrefab, _weaponSocket);

        Vector3 position = defualtWeapon.WeaponData.DefaultPosition;
        Vector3 rotation = defualtWeapon.WeaponData.DefaultLocalEulerAngles;
        weaponGameObject.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(rotation));

        // Refresh the current weapon list from inventory
        var weaponSlots = _equippedWeaponSlots;
        if (weaponSlots.Count == 0)
        {
            Debug.LogWarning("No weapons available to equip after adding default weapon.");
            return;
        }

        _currentWeaponIndex = 0;
        ActivateWeapon(_currentWeaponIndex);
    }

    // Get the command to check wether it's a scroll or a direct selection, and call the corrusponding method
    private void SwitchWeapon(WeaponSwitchCommand command)
    {
        switch (command.Type)
        {
            case WeaponSwitchType.ScrollUp:
                ScrollWeapon(1);
                break;
            case WeaponSwitchType.ScrollDown:
                ScrollWeapon(-1);
                break;
            case WeaponSwitchType.DirectSelect:
                SelectWeaponByIndex(command.Index);
                break;
        }
        _inventoryManager.AddItemToInventory(_defaultWeaponData, 1);

        Debug.Log(_currentWeaponIndex);

    }

    // Direct weapon selection
    private void SelectWeaponByIndex(int index)
    {
        var weapons = _equippedWeaponSlots;
        if (index < 0 || index >= weapons.Count) return;

        // Only activate the weapon if the index has changed
        if (_currentWeaponIndex != index)
        {
            ActivateWeapon(_currentWeaponIndex);
        }
    }

    // Weapon switching by scrolling
    private void ScrollWeapon(int direction)
    {
        var weapons = _equippedWeaponSlots;
        if (weapons.Count == 0) return;

        _currentWeaponIndex = (_currentWeaponIndex + direction + weapons.Count) % weapons.Count;
        Debug.Log(_currentWeaponIndex);
        ActivateWeapon(_currentWeaponIndex);
    }

    private void ActivateWeapon(int index)
    {
        /* Debug.Log(_weapons.Count + "len");

         // Activate only if there is a weapon in the given index
         if (_weapons.Count <= index)
             return;

         if (ActiveWeapon != null)
         {
             ActiveWeapon.Unequip();
             Debug.Log(ActiveWeapon);
         }
         ActiveWeapon = _weapons[index];
         ActiveWeapon.Equip();
         OnWeaponChanged?.Invoke(ActiveWeapon);*/

        var weaponSlots = _equippedWeaponSlots;
        if (index >= weaponSlots.Count)
            return;

        var itemData = weaponSlots[index].ItemData;
        if (itemData.ItemPrefab == null)
        {
            Debug.LogWarning($"Weapon prefab missing for item: {itemData.ItemName}");
            return;
        }

        if (ActiveWeapon != null)
        {
            ActiveWeapon.Unequip();
            Destroy(ActiveWeapon.gameObject); // Optionally pool this instead
        }

        GameObject weaponObj = Instantiate(itemData.ItemPrefab, _weaponSocket);
        Weapon newWeapon = weaponObj.GetComponent<Weapon>();
        if (newWeapon == null)
        {
            Debug.LogError($"Item prefab {itemData.name} does not contain a Weapon component.");
            return;
        }

        weaponObj.transform.SetLocalPositionAndRotation(
            newWeapon.WeaponData.DefaultPosition,
            Quaternion.Euler(newWeapon.WeaponData.DefaultLocalEulerAngles)
        );

        ActiveWeapon = newWeapon;
        ActiveWeapon.Equip();
        OnWeaponChanged?.Invoke(ActiveWeapon);

    }
}