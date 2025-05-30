using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashHUD : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI chargesText;

    private Dash _dash;
    private bool _isUpdating;
    private Coroutine _cooldownCoroutine;

    void Awake()
    {
        _dash = GetComponentInParent<Dash>();
    }

    private void OnEnable()
    {
        _dash.OnDashRecovery += UpdateDashHUD;
    }

    private void OnDisable()
    {
        _dash.OnDashRecovery -= UpdateDashHUD;
    }
    private void Start()
    {
        UpdateChargesText(0);
    }

    private void UpdateChargesText(int currentCharges)
    {
        chargesText.text = currentCharges.ToString();
    }

    private void UpdateDashHUD(float cooldown, int currentCharges)
    {
        UpdateChargesText(currentCharges);
        if (_cooldownCoroutine != null)
            StopCoroutine(_cooldownCoroutine);

        _cooldownCoroutine = StartCoroutine(FillCooldownBar(cooldown, currentCharges));
    }

    private IEnumerator FillCooldownBar(float cooldown, int currentCharges)
    {
        _isUpdating = true;
        float elapsed = 0f;
        fillImage.fillAmount = 0f;

        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = elapsed / cooldown;
            yield return null;
        }

        _isUpdating = false;
        fillImage.fillAmount = 1f;
        UpdateChargesText(currentCharges + 1);
    }
}
