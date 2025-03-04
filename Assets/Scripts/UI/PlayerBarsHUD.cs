using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHUD : MonoBehaviour, IBarsUI
{
    [Header("Bars")]
    [SerializeField] private Image HealthBarFill;
    [SerializeField] private Image ArmorBarFill;
    [SerializeField] private Image ManaBarFill;
    [SerializeField] private Image XpBarFill;
    [SerializeField] private TextMeshProUGUI XpBarText;
    [SerializeField] private TextMeshProUGUI LevelText;

    private Dictionary<string, Image> _bars;
    private Health _playerHealth;
    private PlayerDataManager _playerData;

    private void Awake()
    {
        // Health is a seperate class unlike other bar resources
        _playerHealth = GetComponentInParent<Health>();

        Player player = GetComponentInParent<Player>();
        _playerData = player.PlayerData;

        _bars = new Dictionary<string, Image>
        {
            { "Health", HealthBarFill },
            { "Armor", ArmorBarFill },
            { "Mana", ManaBarFill },
            { "XP", XpBarFill }
        };
    }

    private void OnEnable()
    {
        _playerHealth.OnDamaged += (damageAmount, damageSource) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnHealed += (healAmount) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnKilled += HandleDeath;

        _playerData.OnLevelUp += (level) => UpdateBar("XP", (float)_playerData.Xp / _playerData.XpToLevelUp);
        _playerData.OnXpChanged += (currentXp, requiredXp) => UpdateBar("XP", (float)currentXp / requiredXp);
    }

    private void OnDisable()
    {
        _playerHealth.OnDamaged -= (damageAmount, damageSource) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnHealed -= (healAmount) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnKilled -= HandleDeath;
    }

    // Update bar fill according to ratio
    public void UpdateBar(string barType, float ratio)
    {
        if (_bars.TryGetValue(barType, out Image bar) && bar != null)
        {
            bar.fillAmount = ratio;
        }
    }

    // Update bar fill upon death
    private void HandleDeath(Health health, GameObject killer)
    {
        UpdateBar("Health", 0f);
    }
}