using UnityEngine;

[CreateAssetMenu(fileName = "HitZoneData", menuName = "Combat/Hit Zone Data")]
public class HitZoneData : ScriptableObject
{
    public string ZoneName;

    [Header("Damage")]
    public float DamageMultiplier = 1f;

    [Header("Effects")]
    public GameObject HitVFX;
    public AudioClip HitSound;

    [Header("Gameplay")]
    public bool IsDestructible;
}
