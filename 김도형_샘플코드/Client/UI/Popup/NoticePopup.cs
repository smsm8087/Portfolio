using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticePopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button okButton;

    private Action _onOk;

    public void Init(string message, Action onOk)
    {
        Debug.Log($"[Debug–Client] NoticePopup.Init 호출 → message:\"{message}\", onOk null? {onOk == null}");

        messageText.text = message;
        _onOk = onOk;
        okButton.onClick.AddListener(() => {
            _onOk?.Invoke();
            Close();
        });
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