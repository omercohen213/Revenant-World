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

    private Health _monsterHealth;
    private float _monsterLevel;

    private void Awake()
    {
        _monsterHealth = GetComponentInParent<Health>();

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
            _monsterHealth.OnDamaged += (damageAmount, damageSource) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnHealed += (healAmount) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnKilled += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (_monsterHealth != null)
        {
            _monsterHealth.OnDamaged -= (damageAmount, damageSource) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnHealed -= (healAmount) => UpdateBar("Health", _monsterHealth.GetRatio());
            _monsterHealth.OnKilled -= HandleDeath;
        }
    }

    private void Start()
    {
        MonsterDataManager monsterDataManager = GetComponentInParent<MonsterDataManager>();
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

    private void HandleDeath(Health health, GameObject killer)
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