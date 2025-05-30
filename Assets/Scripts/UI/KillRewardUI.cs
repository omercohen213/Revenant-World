using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillRewardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private Image _xpIcon;
    [SerializeField] private TextMeshProUGUI _kpText;
    [SerializeField] private Image _kpIcon;
    [SerializeField] private TextMeshProUGUI _killText;

    public void Initialize(string killedEntityName, int xp, int kp, float fadeDelay, float fadeDuration)
    {
        _xpText.text = $"+ {xp} XP";
        _kpText.text = $"+ {kp} KP";
        _killText.text = $"Eliminated [{killedEntityName}]";

        gameObject.SetActive(true);
        SetAlpha(1f);
        StartCoroutine(FadeOut(fadeDelay, fadeDuration));
    }

    private IEnumerator FadeOut(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        void SetUIAlpha(Graphic g)
        {
            var c = g.color;
            c.a = alpha;
            g.color = c;
        }

        SetUIAlpha(_xpText);
        SetUIAlpha(_xpIcon);
        SetUIAlpha(_kpText);
        SetUIAlpha(_kpIcon);
        SetUIAlpha(_killText);
    }
}
