using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DataModels;
using TMPro;

public class StatPopup : BasePopup
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI nickText;
    [SerializeField] private TextMeshProUGUI jobText;
    [SerializeField] private Transform cardPanelUpTransform;
    [SerializeField] private Transform cardPanelDownTransform;
    [SerializeField] private Transform statLeftTransform;

    [SerializeField] private GameObject miniCardPrefab;
    [SerializeField] private GameObject statPrefab;
    
    [SerializeField] private GameObject emptyTextObj;
    [SerializeField] private Image playerImage;
    

    private PlayerInfo playerinfo;
    public void Init(PlayerInfo playerinfo)
    {
        this.playerinfo = playerinfo;
        nickText.text = playerinfo.nickName;
        setJobText();
        setCards();
        setStats();
        SetPlayerIcon(null, playerinfo.job_type);
        closeBtn?.onClick.AddListener(OnClose);
    }

    private void setJobText()
    {
        jobText.text = TextManager.Instance.GetText(playerinfo.job_type);
    }

    private void setCards()
    {
        foreach (Transform child in cardPanelUpTransform)
            Destroy(child.gameObject);
        foreach (Transform child in cardPanelDownTransform)
            Destroy(child.gameObject);
        
        var cardTable = GameDataManager.Instance.GetTable<CardData>("card_data");
        for (int i = 0; i < playerinfo.cardIds.Count; i++)
        {
            int cardId =  playerinfo.cardIds[i];
            CardData cardData = cardTable[cardId];
            Transform targetTransform = i > 2 ? cardPanelDownTransform : cardPanelUpTransform;
            var miniCardObj = Instantiate(miniCardPrefab, targetTransform);
            var miniCardSlot = miniCardObj.GetComponent<MiniCardSlot>();
            miniCardSlot.Init(cardData);
        }
        emptyTextObj.SetActive(playerinfo.cardIds.Count == 0);
    }

    private void setStats()
    {
        List<(string,string)> slotTextList =  new List<(string,string)>();
        slotTextList.Add((TextManager.Instance.GetText("attack_power"), playerinfo.currentAttack.ToString()));
        slotTextList.Add((TextManager.Instance.GetText("attack_speed"), playerinfo.currentAttackSpeed.ToString("F2")));
        slotTextList.Add((TextManager.Instance.GetText("hp"), playerinfo.currentMaxHp.ToString()));
        slotTextList.Add((TextManager.Instance.GetText("move_speed"), playerinfo.currentMoveSpeed.ToString()));
        slotTextList.Add((TextManager.Instance.GetText("ultgauge"), playerinfo.currentUltGauge.ToString()));
        slotTextList.Add((TextManager.Instance.GetText("cri_pct"), playerinfo.currentCriPct.ToString()));
        slotTextList.Add((TextManager.Instance.GetText("cri_dmg"), playerinfo.currentCriDmg.ToString()));
        
        for (int i = 0; i < slotTextList.Count; i++)
        {
            GameObject statObj = Instantiate(statPrefab, statLeftTransform);
            StatSlot slot =  statObj.GetComponent<StatSlot>();
            if (!slot) continue;
            slot.Init(slotTextList[i].Item1, slotTextList[i].Item2);
        }
    }
    private void OnClose()
    {
        if (ui_lock) return;
        ui_lock = true;
        Close();
    }
    public void SetPlayerIcon(Sprite playerSprite, string jobType = "")
    {
        // 항상 jobType 기반으로 로딩
        if (!string.IsNullOrEmpty(jobType))
        {
            string capitalJob = FirstCharToUpper(jobType);
            string spritePath = $"Character/{capitalJob}/PROFILE_{capitalJob}";

            Sprite overrideSprite = Resources.Load<Sprite>(spritePath);
            Debug.Log($"[ProfileUI] Try load sprite: {spritePath} => {(overrideSprite != null ? "Success" : "Fail")}");
            if (overrideSprite != null)
            {
                playerSprite = overrideSprite;
            }
            else
            {
                Debug.LogWarning($"[ProfileUI] Sprite '{spritePath}'을(를) Resources에서 찾지 못함");
                return;
            }
        }
        else if (playerSprite == null)
        {
            Debug.LogWarning("playerSprite도 없고 jobType도 없음");
            return;
        }

        playerImage.sprite = playerSprite;
    }
    private string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}

