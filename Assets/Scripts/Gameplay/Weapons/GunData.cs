using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Inventory/Gun Data")]
public class GunData : RangedWeaponData
{
    [Header("Shooting")]
    public Gun.ShootType ShootType;
    public Bullet BulletPrefab;
    public float Damage;
    public float DelayBetweenShots = 0.1f;
    public int BulletsPerShot = 1;
    //public float BulletSpreadAngle = 0f;
    public float BaseSpread = 0.05f;          // Default spread when standing still
    public float SpreadIncreasePerShot = 0.01f; // Added spread per shot fired
    public float MaxSpread = 0.2f;           // Maximum allowed spread

    [Header("Aiming")]
    public float AimZoomRatio = 1.5f;
    public Vector3 AimOffset;
    public float AimingAnimationSpeed = 10f;

    [Header("Recoil")]
    public float RecoilForce = 1.0f;
    public float MaxRecoilDistance = 0.5f;
    public float RecoilSharpness = 50f;
    public float RecoilRestitutionSharpness = 10f;
    public float RecoilRecoverySpeed = 0.1f;

    [Header("Bobbing")]
    public float BobFrequency = 10f;
    public float BobSharpness = 10f;
    public float DefaultBobAmount = 0.05f;
    public float AimingBobAmount = 0.02f;    
}
