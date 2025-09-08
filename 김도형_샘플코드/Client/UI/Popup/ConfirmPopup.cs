using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    public void Init(string message, Action onYes, Action onNo)
    {
        messageText.text = message;
        yesButton.onClick.AddListener(() => { onYes?.Invoke(); Close(); });
        noButton .onClick.AddListener(() => { onNo ?.Invoke(); Close(); });
    }

    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        Destroy(gameObject);
    }
}