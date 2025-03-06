using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsHUD : MonoBehaviour
{
    [Required][SerializeField] private TextMeshProUGUI _xpText;
    [Required][SerializeField] private Image _xpIcon;
    [Required][SerializeField] private TextMeshProUGUI _kpText;
    [Required][SerializeField] private Image _kpIcon;
    [Required][SerializeField] private TextMeshProUGUI _killText;

    [SerializeField] private float _fadeDelay = 1f;
    [SerializeField] private float _fadeDuration = 1f;
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        if (_player != null)
        {
            _player.OnKillRewarded += ShowRewards;
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnKillRewarded -= ShowRewards;
        }
    }

    // Activate the rewards objects on the player hud
    private void ShowRewards(GameEntity killedEntity, int xp, int kp)
    {
        string killedEntityName = killedEntity.GetEntityData().baseData.name;

        SetActive(true);
        SetAlpha(1f);
        SetTexts(killedEntityName, xp, kp);

        StartCoroutine(FadeOutRewards(_fadeDelay, _fadeDuration)); // Wait 3s, fade out over 1s
    }

    private void SetActive(bool active)
    {
        _xpText.gameObject.SetActive(active);
        _xpIcon.gameObject.SetActive(active);
        _kpText.gameObject.SetActive(active);
        _kpIcon.gameObject.SetActive(active);
        _killText.gameObject.SetActive(active);
    }


    // Set the correct texts according to the rewards
    private void SetTexts(string killedEntityName, int xp, int kp)
    {
        _xpText.text = $"+ {xp} XP";
        _kpText.text = $"+ {kp} KP";
        _killText.text = $"Eliminated [{killedEntityName}]";
    }

    // Fade the rewards by chaning the color alpha
    private IEnumerator FadeOutRewards(float fadeDelay, float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDelay); // Wait before fading out

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        Color xpColor = _xpText.color;
        Color xpIconColor = _xpIcon.color;
        Color kpColor = _kpText.color;
        Color kpIconColor = _kpIcon.color;
        Color killColor = _killText.color;

        xpColor.a = alpha;
        xpIconColor.a = alpha;
        kpColor.a = alpha;
        kpIconColor.a = alpha;
        killColor.a = alpha;

        _xpText.color = xpColor;
        _xpIcon.color = xpIconColor;
        _kpText.color = kpColor;
        _kpIcon.color = kpIconColor;
        _killText.color = killColor;
    }

}
