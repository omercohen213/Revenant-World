using UnityEngine;
using UnityEngine.UI;  // Required for UI components like Image and Slider
using UnityEngine.Events;

public class HealthHUD : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The health bar fill (e.g., a UI Image or Slider)")]
    public Image healthBarFill;  // Use Image component for a fillable bar

    private Health playerHealth;

    private void Start()
    {
        // Find the player health component
        playerHealth = GetComponentInParent<Entity>().GetComponent<Health>();

        if (playerHealth == null)
        {
            Debug.LogError("Health component not found on parent entity.");
            return;
        }

        // Subscribe to health-related events with a matching signature
        playerHealth.OnDamaged += (damageAmount, damageSource) => UpdateHealthBar(playerHealth.GetRatio());
        playerHealth.OnHealed += (healAmount) => UpdateHealthBar(healAmount); // Set damageSource to null for healing

        playerHealth.OnDie += HandleDeath;

        // Initialize the health bar at start
        InitializeHealthBar();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnDamaged -= (damageAmount, damageSource) => UpdateHealthBar(playerHealth.GetRatio());
            playerHealth.OnHealed -= UpdateHealthBar;
            playerHealth.OnDie -= HandleDeath;
        }
    }

    private void InitializeHealthBar()
    {
        // Set the initial health bar state based on current health
        UpdateHealthBar(playerHealth.MaxHealth);
    }

    private void UpdateHealthBar(float healthRatio)
    {
        // Update the health bar's fill amount based on the current health ratio
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = healthRatio;  // Fill amount goes from 0 to 1
        }
    }

    private void HandleDeath()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 0f;
        }

        // You can trigger death UI (e.g., game over screen) here.
    }
}
