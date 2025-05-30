using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _inMagAmmo;
    [SerializeField] private TextMeshProUGUI _outMagAmmo;
    [SerializeField] private Image _ammoIcon;
    [SerializeField] private Image _weaponIcon;

    private InventoryManager _inventoryManager;
    private WeaponManager _weaponManager;
    private Weapon _activeWeapon;

    private void Awake()
    {
        _inventoryManager = GetComponentInParent<InventoryManager>();
        _weaponManager = GetComponentInParent<WeaponManager>();
    }

    private void OnEnable()
    {
        _weaponManager.OnWeaponChanged += ChangeWeaponHUD;

        if (_activeWeapon is RangedWeapon rangedWeapon)
        {
            // HUD ammo updates are needed only for non infinite ammo weapon
            if (!rangedWeapon.RangedWeaponData.HasInfiniteAmmo)
            {
                rangedWeapon.OnShoot += UpdateAmmoText;
                rangedWeapon.OnReload += UpdateAmmoText;
            }
        }
    }

    private void OnDisable()
    {
        _weaponManager.OnWeaponChanged -= ChangeWeaponHUD;

        if (_activeWeapon is RangedWeapon rangedWeapon)
        {
            // HUD ammo updates are needed only for non infinite ammo weapon
            if (!rangedWeapon.RangedWeaponData.HasInfiniteAmmo)
            {
                rangedWeapon.OnShoot -= UpdateAmmoText;
                rangedWeapon.OnReload -= UpdateAmmoText;
            }
        }
    }

    // Adjust the weapon HUD when changing a weapon and update the active weapon
    private void ChangeWeaponHUD(Weapon newWeapon)
    {
        if (_activeWeapon is RangedWeapon oldRanged)
        {
            oldRanged.OnShoot -= UpdateAmmoText;
            oldRanged.OnReload -= UpdateAmmoText;
        }

        _activeWeapon = newWeapon;

        if (_activeWeapon is RangedWeapon newRanged)
        {
            newRanged.OnShoot += UpdateAmmoText;
            newRanged.OnReload += UpdateAmmoText;
            ShowAmmo();
            UpdateAmmoText();
            UpdateWeaponIcon();
        }
        else
        {
            HideAmmo();
        }
    }

    // Update ammo text according to the ammo the player has for the active weapon
    private void UpdateAmmoText()
    {
        if (_activeWeapon == null || _inMagAmmo == null || _outMagAmmo == null)
            return;

        RangedWeapon rangedWeapon = _activeWeapon as RangedWeapon;
        if (rangedWeapon.RangedWeaponData.HasInfiniteAmmo)
        {
            _inMagAmmo.text = "\u221E";  // Unicode for infinity symbol
            _outMagAmmo.text = "\u221E";
        }
        else
        {
            _inMagAmmo.text = rangedWeapon.CurrentAmmo.ToString();
            _outMagAmmo.text = _inventoryManager.GetTotalQuantityOfItem(rangedWeapon.RangedWeaponData.RequiredAmmo).ToString();
        }
    }

    // Update wepaon sprite
    private void UpdateWeaponIcon()
    {
        Sprite weaponSprite = _activeWeapon.ItemData.Icon;
        _weaponIcon.sprite = weaponSprite;
        RectTransform rectTransform = _weaponIcon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(150, 150);
    }

    // Update ammo sprite of a ranged weapon
    private void UpdateAmmoIcon()
    {
        RangedWeapon rangedWeapon = _activeWeapon as RangedWeapon;
        Sprite ammoSprite = rangedWeapon.RangedWeaponData.RequiredAmmo.Icon;
        _ammoIcon.sprite = ammoSprite;
    }

    // Show ammo text and icon  
    private void ShowAmmo()
    {
        if (_inMagAmmo != null) _inMagAmmo.gameObject.SetActive(true);
        if (_outMagAmmo != null) _outMagAmmo.gameObject.SetActive(true);
    }

    // Hide ammo text and icon  
    private void HideAmmo()
    {
        if (_inMagAmmo != null) _inMagAmmo.gameObject.SetActive(false);
        if (_outMagAmmo != null) _outMagAmmo.gameObject.SetActive(false);
    }
}
