using TMPro;
using UnityEngine;

public class StatSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private TextMeshProUGUI statValueText;

    public void Init(string text, string value)
    {
        statText.text = text;
        statValueText.text = value;
    }
}
