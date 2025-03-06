using UnityEngine;

public interface IBarsUI
{
    void UpdateBar(string barType, float ratio);
    void UpdateText(string textType, string text);
}
