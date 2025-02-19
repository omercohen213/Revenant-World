using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHUD : MonoBehaviour
{
    [Header("Health Bar")]
    public Image HealthBarFill;

    [Header("Shield Bar")]
    //public Image ArmorBarFill;

    [Header("Xp Bar")]
    public Image XpBarFill;
    public TextMeshProUGUI XpBarText;
    public TextMeshProUGUI LevelText;

    private Health _playerHealth;
    private Player _player;
    private PlayerDataManager _playerData;

    private void Awake()
    {
        _playerHealth = GetComponentInParent<Health>();
        _player = GetComponentInParent<Player>();
        _playerData = _player.PlayerData;
    }

    private void Start()
    {   
        InitializeHealthBar();
        InitializeArmorBar();
        InitializeXpBar();
        InitializeLevelText();
    }

  
    private void OnEnable()
    {
        _playerHealth.OnDamaged += (damageAmount, damageSource) => UpdateHealthBar(_playerHealth.GetRatio());
        _playerHealth.OnHealed += (healAmount) => UpdateHealthBar(healAmount);
        _playerHealth.OnKilled += HandleDeath;

        _playerData.OnLevelUp += (level) => UpdateLevelText(level);
        _playerData.OnLevelUp += (level) => UpdateXpBar(_playerData.Xp, _playerData.XpToLevelUp);
        _playerData.OnXpChanged += (currentXp, requiredXp) => UpdateXpBar(currentXp, requiredXp);
    }

    private void OnDisable()
    {
        _playerHealth.OnDamaged -= (damageAmount, damageSource) => UpdateHealthBar(_playerHealth.GetRatio());
        _playerHealth.OnHealed -= UpdateHealthBar;
        _playerHealth.OnKilled -= HandleDeath;
    }

    private void InitializeHealthBar()
    {
        UpdateHealthBar(_playerHealth.MaxHealth);
    }

    private void UpdateHealthBar(float healthRatio)
    {
        if (HealthBarFill != null)
        {
            HealthBarFill.fillAmount = healthRatio; 
        }
    }   

    private void InitializeArmorBar()
    {
    }

    private void InitializeXpBar()
    {
        int xpToLevelUp = _playerData.XpToLevelUp;
        UpdateXpBar(0, xpToLevelUp);
    }

    private void InitializeLevelText()
    {
        LevelText.text = "1";
    }

    private void UpdateXpBar(int currentXp, int xpToLevelUp)
    {
        if (XpBarFill != null)
        {
            XpBarFill.fillAmount = (float) currentXp / xpToLevelUp;
        }
        if (XpBarText != null)
        {
            XpBarText.text = $"{currentXp} / {xpToLevelUp} XP";            
        }
    }

    private void UpdateLevelText(int level)
    {
        if (LevelText != null)
        {
            LevelText.text = level.ToString();
        }
    }

    private void HandleDeath(Health health, GameObject killer)
    {
        if (HealthBarFill != null)
        {
            HealthBarFill.fillAmount = 0f;
        }
    }

}
