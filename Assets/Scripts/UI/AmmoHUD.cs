using DevionGames.InventorySystem;
using TMPro;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using Unity.FPS.UI;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHUD: MonoBehaviour
{
    [Tooltip("Text for Bullet Counter in Current Magazine")]
    public TextMeshProUGUI InMagAmmo;

    [Tooltip("Text for Bullet Counter out of Current Magazine")]
    public TextMeshProUGUI OutMagAmmo;

    private InventoryManager _inventoryManager;
    private Weapon _activeWeapon;

    void Awake()
    {
        DebugUtil.SafeGetComponentInParent(gameObject, out _inventoryManager);
        _activeWeapon = _inventoryManager.GetActiveWeapon();
    }

    private void OnEnable()
    {
        _activeWeapon.OnShoot += UpdateAmmoText;
        _activeWeapon.OnReload += UpdateAmmoText;
    }

    private void Start()
    {
        UpdateAmmoText(); // Ensure UI is correct at start
    }

    void OnDisable()
    {
        // Unsubscribe from events to avoid memory leaks
        if (_activeWeapon != null)
        {
            //_weapon.OnShootProcessed -= UpdateAmmoText;
            _activeWeapon.OnShoot -= UpdateAmmoText;
            _activeWeapon.OnReload -= UpdateAmmoText;
        }
    }

    /// <summary>
    /// Updates the UI with the current ammo count.
    /// </summary>
    private void UpdateAmmoText()
    {
        if (_activeWeapon == null || InMagAmmo == null || OutMagAmmo == null)
            return;

        InMagAmmo.text = _activeWeapon.CurrentAmmo.ToString();
        OutMagAmmo.text = _inventoryManager.GetTotalAmmo(_activeWeapon).ToString();
    }

    /// <summary>
    /// Called when the player reloads.
    /// </summary>
    public void OnReload()
    {
        UpdateAmmoText();
    }

    /// <summary>
    /// Called when the player shoots.
    /// </summary>
    public void OnShoot()
    {
        UpdateAmmoText();
    }

    /// <summary>
    /// Called when the player changes weapon.
    /// </summary>
    private void OnWeaponChange(Weapon weapon)
    {

    }
}


