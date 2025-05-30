using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;


public class WeaponManager : MonoBehaviour
{
    public Weapon ActiveWeapon;

    [SerializeField] private WeaponData _defaultWeaponData;
    [SerializeField] private Transform _weaponSocket; // Parent of the weapon to instantiate into

    private List<Weapon> _weapons;
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
        _weapons = new List<Weapon>();
    }

    void Start()
    {
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
        GameObject defaultWeaponPrefab = _defaultWeaponData.ItemPrefab;
        Weapon defualtWeapon = defaultWeaponPrefab.GetComponent<Weapon>();
        GameObject weaponGameObject = Instantiate(defaultWeaponPrefab, _weaponSocket);

        Vector3 position = defualtWeapon.WeaponData.DefaultPosition;
        Vector3 rotation = defualtWeapon.WeaponData.DefaultLocalEulerAngles;
        weaponGameObject.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(rotation));

        _currentWeaponIndex = 0;
        _weapons.Add(defualtWeapon);
        ActivateWeapon(_currentWeaponIndex);
        OnWeaponChanged?.Invoke(_weapons[_currentWeaponIndex]);
    }

    // Get index of the numpad pressed (1-9) and change the current weapon based on it
    private void SwitchWeapon(int indexChange)
    {
        int newWeaponIndex = _currentWeaponIndex;

        if (indexChange > 0) // Scrolling up or selecting next weapon
        {
            newWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Count;
        }
        else if (indexChange < 0) // Scrolling down or selecting previous weapon
        {
            newWeaponIndex = _currentWeaponIndex - 1;
            if (newWeaponIndex < 0)
                newWeaponIndex = _weapons.Count - 1;
        }
        else if (indexChange >= 1 && indexChange <= _weapons.Count) // Direct selection (1-4)
        {
            newWeaponIndex = indexChange - 1;
        }

        // Only activate the weapon if the index has changed
        if (newWeaponIndex != _currentWeaponIndex)
        {
            _currentWeaponIndex = newWeaponIndex;
            ActivateWeapon(_currentWeaponIndex);
        }
    }

    private void ActivateWeapon(int index)
    {
        if (ActiveWeapon != null)
        {
            ActiveWeapon.Unequip();
            Debug.Log(ActiveWeapon);
        }
        ActiveWeapon = _weapons[index];
        ActiveWeapon.Equip();
        OnWeaponChanged?.Invoke(ActiveWeapon);
    }
}