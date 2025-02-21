using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;

    [SerializeField] private Vector3 _offset = new(0, 1.61f, 0); // change offset to match object size relatively
    [SerializeField] private int _size;
    private Color _color;

    private void OnEnable()
    {
        transform.position = gameObject.transform.position + _offset;
        _textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FloatingTextAnimation());
    }

    public void SetText(string textToDisplay)
    {
        TextMeshProUGUI textMesh= gameObject.GetComponent<TextMeshProUGUI>();
        _textMesh = textMesh;
        textMesh.text = textToDisplay;
    }

    private IEnumerator FloatingTextAnimation()
    {
        float timeAlive = 0f;
        float fadeTime = 1.5f; // Duration for text to stay on screen
        Vector3 moveDirection = Vector3.up * 0.5f;

        // Animate the text to move upwards and fade out over time
        while (timeAlive < fadeTime)
        {
            gameObject.transform.position += moveDirection * Time.deltaTime;
            _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, 1 - (timeAlive / fadeTime));
            timeAlive += Time.deltaTime;
            yield return null;
        }

        // Disable the damage text after it has faded
        gameObject.SetActive(false);
    }
}
