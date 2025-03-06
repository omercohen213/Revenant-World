using NaughtyAttributes;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBarsHUD : MonoBehaviour, IBarsUI
{
    [Header("Bars")]

    [Required][SerializeField] private Image _healthBarFill;
    [SerializeField] private TextMeshProUGUI _healthBarText;
    [Required][SerializeField] private Image _xpBarFill;
    [Required][SerializeField] private TextMeshProUGUI _xpBarText;
    [Required][SerializeField] private TextMeshProUGUI _levelText;

    [SerializeField] private Image _armorBarFill;
    [SerializeField] private Image _manaBarFill;

    private Dictionary<string, Image> _bars;
    private Dictionary<string, TextMeshProUGUI> _texts;
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
            { "Health", _healthBarFill },
            { "Armor", _armorBarFill },
            { "Mana", _manaBarFill },
            { "XP", _xpBarFill }
        };

        _texts = new Dictionary<string, TextMeshProUGUI>
        {
            { "Health", _healthBarText },
            { "Level", _levelText },
            { "XP", _xpBarText }
        };
    }

    private void OnEnable()
    {
        _playerHealth.OnLostHealth += (damageAmount, damageSource) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnGainedHealth += (healAmount) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnHealthReachedZero += HandleDeath;

        _playerData.OnLevelUp += (level) => UpdateBar("XP", (float)_playerData.Xp / _playerData.XpToLevelUp);
        _playerData.OnLevelUp += (level) => UpdateText("Level", level.ToString());
        _playerData.OnXpChanged += (currentXp, requiredXp) => UpdateBar("XP", (float)currentXp / requiredXp);
        _playerData.OnXpChanged += (currentXp, requiredXp) => UpdateText("XP", $"{currentXp}/{requiredXp}");
    }

    private void OnDisable()
    {
        _playerHealth.OnLostHealth -= (damageAmount, damageSource) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnGainedHealth -= (healAmount) => UpdateBar("Health", _playerHealth.GetRatio());
        _playerHealth.OnHealthReachedZero -= HandleDeath;
    }

    private void Start()
    {
        InitializeHealthBar();
        InitializeArmorBar();
        InitializeXpBar();

        string startingLevelText = "1";
        UpdateText("Level", startingLevelText);
    }

    private void InitializeHealthBar()
    {
        UpdateBar("Health", _playerHealth.MaxHealth);
    }

    private void InitializeArmorBar()
    {

    }

    private void InitializeXpBar()
    {
        int currentXp = 0;
        int requiredXp = _playerData.XpToLevelUp;
        UpdateBar("XP", currentXp / requiredXp);
    }

    // Update a bar fill according to ratio
    public void UpdateBar(string barType, float ratio)
    {
        if (_bars.TryGetValue(barType, out Image bar) && bar != null)
        {
            bar.fillAmount = ratio;
        }
    }

    // Update a text
    public void UpdateText(string textType, string text)
    {
        if (_texts.TryGetValue(textType, out TextMeshProUGUI textMesh) && textMesh != null)
        {
            textMesh.text = text;
        }
    }

    // Update bar fill upon death
    private void HandleDeath(Health health, GameObject killer)
    {
        UpdateBar("Health", 0f);
    }
}