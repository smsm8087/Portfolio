using DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniCardSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image Icon;
    [SerializeField] private Image border;

    private CardData cardData;
    private string baseResourcePath = "UI/skillcards_ui/";

    public void Init(CardData cardData)
    {
        this.cardData = cardData;
    
        setCard();
        setIconType();
    }
    public void setIconType()
    {
        string type = cardData.type;
        switch (type)
        {
            case "add_criticalpct":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_criticalpercentageup");
                break;
            case "add_attack":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_damageup");
                break;
            case "add_movespeed":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_movspeedup");
                break;
            case "add_ultgauge":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_ultgaugeup");
                break;
            case "add_criticaldmg":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_criticaldamageup");
                break;
            case "add_hp":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_healthicon");
                break;
        }
    }
    public void setCard()
    {
        string grade = cardData.grade;
        int value = cardData.value;
        string displayValue = cardData.need_percent == 1 ? $"{value}%" : value.ToString();
        
        switch (grade)
        {
            case "normal":
                border.sprite = Resources.Load<Sprite>(baseResourcePath + "NORMAL_SKILLCARD");
                break;
            case "rare":
                border.sprite = Resources.Load<Sprite>(baseResourcePath + "EPIC_SKILLCARD");
                break;
            case "legend":
                border.sprite = Resources.Load<Sprite>(baseResourcePath + "LEGENDARY_SKILLCARD");
                break; 
        }
        valueText.text = $"+ {displayValue}";
    }
}
