using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject TextPrefab;

    public static FloatingTextManager Instance;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowFloatingText(GameObject parentGo, Vector3 position, string textToDisplay)
    {
        if (parentGo.GetComponentInChildren<Canvas>() != null)
        {
            Canvas parentCanvas = parentGo.GetComponentInChildren<Canvas>();
            if (parentCanvas.transform.Find("FloatingTexts") != null)
            {
                Transform parentTransform = parentCanvas.transform.Find("FloatingTexts");
                GameObject textObject = Instantiate(TextPrefab, position, Quaternion.identity, parentTransform);
                FloatingText floatingText = textObject.GetComponentInChildren<FloatingText>();

                floatingText.SetText(textToDisplay);
            }
            else {
                Debug.Log("No FloatingTexts parent transform");
            }
        }
        else
        {
            Debug.Log("No FloatingTexts parent Canvas");
        }       
    }

    
}
