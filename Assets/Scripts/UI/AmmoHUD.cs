using TMPro;
using UnityEngine;

public class AmmoHUD: MonoBehaviour
{
    [Tooltip("Text for Bullet Counter in Current Magazine")]
    public TextMeshProUGUI InMagAmmo;

    [Tooltip("Text for Bullet Counter out of Current Magazine")]
    public TextMeshProUGUI OutMagAmmo;

    private InventoryManager _inventoryManager;
    private RangedWeapon _activeWeapon;

    void Awake()
    {
        DebugUtil.SafeGetComponentInParent(gameObject, out _inventoryManager);
        DebugUtil.SafeGetComponentInParent(gameObject, out Player player);
    
        _activeWeapon = (RangedWeapon) player.ActiveWeapon; 
    }

    private void OnEnable()
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.OnShoot += UpdateAmmoText;
            _activeWeapon.OnReload += UpdateAmmoText;
        }
        else
        {
            Debug.LogWarning("Active weapon is not assigned.");
        }
    }

    private void Start()
    {
        UpdateAmmoText(); // Ensure UI is correct at start
    }

    void OnDisable()
    {
        if (_activeWeapon != null)
        {
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


