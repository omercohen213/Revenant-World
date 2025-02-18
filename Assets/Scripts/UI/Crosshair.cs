using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [System.Serializable]
    public struct CrosshairData
    {
        [Tooltip("The image that will be used for this weapon's crosshair")]
        public Sprite CrosshairSprite;

        [Tooltip("The size of the crosshair image")]
        public int CrosshairSize;

        [Tooltip("The color of the crosshair image")]
        public Color CrosshairColor;
    }

    WeaponManager _weaponManager;
    InventoryManager _inventoryManager;

    void Start()
    {
        DebugUtil.SafeGetComponentInParent(gameObject, out _weaponManager);
        DebugUtil.SafeGetComponentInParent(gameObject, out _inventoryManager);
    }

    void Update()
    {
    }

    public void DisableCrosshair()
    {
        gameObject.SetActive(false);
    }

    public void EnableCrosshair()
    {
        gameObject.SetActive(true);
    }

}
