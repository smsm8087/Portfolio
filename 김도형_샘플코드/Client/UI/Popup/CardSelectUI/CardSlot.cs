using System.Collections;
using System.Collections.Generic;
using DataModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image highlight;
    [SerializeField] private Image Icon;
    [SerializeField] private Image border;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private CardData cardData;
    private CardData cardClientData;
    private System.Action<int> onClick;
    // private string baseResourcePath = "UI/skillcards_ui/";

    private bool isDuringAction = true;
    private bool isUILocked = false;
    
    public void Init(CardData cardData,  System.Action<int> onClickCallback)
    {
        this.cardData = cardData;
        cardClientData = GameDataManager.Instance.GetData<CardData>("card_data", cardData.id);
        setCard();
        setIconType();
        
        onClick = onClickCallback;
        SetSelected(false);
        canvasGroup.alpha = 0f;
        isUILocked = false;
    }

    public void StartCoroutine(float waitTime)
    {
        StartCoroutine(StartAnimation(waitTime));
    }
    private IEnumerator StartAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);  
        float duration = 0.3f;
        float time = 0f;

        Vector3 startPos = transform.localPosition;
        Vector3 from = startPos + new Vector3(-100f, 0f, 0f); // 왼쪽 오프셋
        Vector3 to = startPos;

        transform.localPosition = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            float eased = Utils.EaseOutCubic(t); 
            transform.localPosition = Vector3.Lerp(from, to, eased);
            
            canvasGroup.alpha = eased;

            yield return null;
        }

        transform.localPosition = to;
        canvasGroup.alpha = 1f;
        isDuringAction = false;
    }

    public void OnClick()
    {
        if (isDuringAction || isUILocked) return;
        onClick?.Invoke(cardData.id);
    }
    
    public void SetUILock(bool locked)
    {
        isUILocked = locked;
    }

    public void SetSelected(bool selected)
    {
        if (highlight != null)
            highlight.enabled = selected;
    }

    //TODO : 아이콘 작업 끝나면 작업 예정
    public void setIconType()
    {
        Icon.sprite = Resources.Load<Sprite>(cardClientData.icon_path);
    }
    public void setCard()
    {
        string title = TextManager.Instance.GetText(cardClientData.title);
        int value = cardClientData.value;
        string valueText;
        if (cardClientData.need_percent == 1)
        {
            valueText = $"{value}%";
        }
        else if (cardClientData.type == "add_attackspeed")
        {
            float displayValue = value / 10.0f;
            valueText = displayValue.ToString("F1");  // 2 → "0.2"
        }
        else
        {
            valueText = value.ToString();
        }
        border.sprite = Resources.Load<Sprite>(cardClientData.border_path);
        string colorhex = cardClientData.color;
        if (ColorUtility.TryParseHtmlString(colorhex, out Color gradeColor))
        {
            cardNameText.text = $"{title}\n<color={colorhex}><size=50>{valueText}</color>";
        }
    }
}