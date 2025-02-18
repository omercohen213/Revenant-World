using UnityEngine;

public class PlayerHealth : Health
{
    [Tooltip("Health ratio at which the critical health vignette starts appearing")]
    public float CriticalHealthRatio = 0.3f;

    public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

    protected override void Start()
    {
        MaxHealth = GetComponent<PlayerData>().baseData.MaxHealth;
        base.Start();
    }
}
