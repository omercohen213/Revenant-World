using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBarsUI : MonoBehaviour, IBarsUI
{
    private Image HealthBarFill;

    private Dictionary<string, Image> _bars;
    private Health _monsterHealth;

    private void Awake()
    {
        _monsterHealth = GetComponentInParent<Health>();
        Transform healthBarTransform = transform.Find("HealthBar");
        HealthBarFill = healthBarTransform.Find("Fill").GetComponent<Image>();

        _bars = new Dictionary<string, Image>
        {
            { "Health", HealthBarFill }
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
}