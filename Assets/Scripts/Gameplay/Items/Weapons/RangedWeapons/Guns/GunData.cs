using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Items/Gun Data")]
public class GunData : RangedWeaponData
{
    [Header("Shooting")]
    public Gun.ShootType ShootType;
    public GameObject MuzzleFlashPrefab;
    public int BulletsPerShot = 1;
    //public float BulletSpreadAngle = 0f;
    public float BaseSpread = 0.05f;          // Default spread when standing still
    public float SpreadIncreasePerShot = 0.01f; // Added spread per shot fired
    public float MaxSpread = 0.2f;           // Maximum allowed spread

    [Header("Recoil")]
    public float RecoilForce = 1.0f;
    public float MaxRecoilDistance = 0.5f;
    public float RecoilSharpness = 50f;
    public float RecoilRestitutionSharpness = 10f;
    public float RecoilRecoverySpeed = 0.1f;
}
