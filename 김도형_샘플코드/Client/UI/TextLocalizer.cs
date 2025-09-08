using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalizer : MonoBehaviour
{
    public string textKey;

    private void Start()
    {
        ApplyLocalizedText();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        GetComponent<TextMeshProUGUI>().text = $"#{textKey}";
    }
#endif

    public void ApplyLocalizedText()
    {
        var textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.text = TextManager.Instance.GetText(textKey);
    }
}