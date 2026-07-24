using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class MonsterBarsUI : MonoBehaviour, IBarsUI
{
    [Required][SerializeField]private Image _healthBarFill;
    [Required][SerializeField]private TextMeshProUGUI _levelText;

    private Dictionary<string, Image> _bars;
    private Dictionary<string, TextMeshProUGUI> _texts;

    private EntityHealth _monsterHealth;
    private float _monsterLevel;

    private void Awake()
    {
        _monsterHealth = GetComponentInParent<EntityHealth>();

        _bars = new Dictionary<string, Image>
        {
            { "Health", _healthBarFill }
        };
        _texts = new Dictionary<string, TextMeshProUGUI>
        {
            { "Level", _levelText}
        };
    }

    private void OnEnable()
    {
        if (_monsterHealth != null)
        {
            _monsterHealth.OnLostHealth += (damageAmount, damageSource) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnGainedHealth += (healAmount) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnHealthReachedZero += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (_monsterHealth != null)
        {
            _monsterHealth.OnLostHealth -= (damageAmount, damageSource) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnGainedHealth -= (healAmount) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnHealthReachedZero -= HandleDeath;
        }
    }

    private void Start()
    {
        MonsterRuntimeData monsterDataManager = GetComponentInParent<MonsterRuntimeData>();
        _monsterLevel = monsterDataManager.Level;
        UpdateText("Level", _monsterLevel.ToString());
    }

    public void UpdateBar(string barType, float ratio)
    {
        if (_bars.TryGetValue(barType, out Image bar) && bar != null)
        {
            bar.fillAmount = ratio;
        }
    }

    private void HandleDeath(EntityHealth health, GameObject killer)
    {
        UpdateBar("Health", 0f);
    }

    public void UpdateText(string textType, string text)
    {
        if (_texts.TryGetValue(textType, out TextMeshProUGUI textMesh) && textMesh != null)
        {
            textMesh.text = text;
        }
    }
}