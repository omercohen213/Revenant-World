using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Collision")]
    public float Radius = 0.03f;
    public LayerMask HittableLayers = -1;

    [Header("Lifetime")]
    public float MaxLifeTime = 5f;

    [Header("Movement")]
    public float Speed = 20f;
    public float GravityAcceleration = 0f;

    public bool InheritWeaponVelocity;

    [Tooltip("Negative means disabled")]
    public float TrajectoryCorrectionDistance = -1;

    [Header("Impact")]
    public GameObject ImpactVFX;
    public float ImpactVFXOffset = 0.1f;
}