using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPText : MonoBehaviour
{
    TextMeshProUGUI hpText;

    private void Awake()
    {
        hpText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateHP(float currentHP, float maxHP)
    {
        if (!hpText) return;
        hpText.text = $"{currentHP}/{maxHP}";
    }
}
