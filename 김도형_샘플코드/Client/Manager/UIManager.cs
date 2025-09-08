using UnityEngine;
using System.Collections.Generic;
using DataModels;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject GameUICanvas;
    [SerializeField] private GameObject IntroUICanvas;
    
    [SerializeField] private GameObject BossHpUI;
    [SerializeField] private GameObject cardSelectPopupPrefab;
    [SerializeField] private GameObject statUIPrefab;
    [SerializeField] private GameObject gameResultPopupPrefab;
    private CardSelectPopup cardSelectPopup;
    private StatPopup statUIPopup;
    private GameResultPoup gameResultPoup;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region canvas
    public void setActiveGameUICanvas(bool isActive)
    {
        GameUICanvas.SetActive(isActive);
    }
    public void setActiveIntroUICanvas(bool isActive)
    {
        IntroUICanvas.SetActive(isActive);
    }
    public void setActiveBossHpUI(bool isActive)
    {
        BossHpUI.SetActive(isActive);
    }

    public GameObject getIntroUICanvas()
    {
        return IntroUICanvas;
    }
    public GameObject getBossHpUI()
    {
        return BossHpUI;
    }
    
    #endregion

    #region cardSelectPopup
    public void ShowCardSelectPopup(List<CardData> cards, float duration, int alivePlayerCount) 
    {
        cardSelectPopup = PopupManager.Instance.ShowPopup<CardSelectPopup>(cardSelectPopupPrefab);
        cardSelectPopup.Init(cards, duration, alivePlayerCount); 
    }

    public void UpdateSettlementTimer(float duration)
    {
        cardSelectPopup?.UpdateTimer(duration);
    }
    public void UpdateSettlementReadyCount(int readyCount)
    {
        cardSelectPopup?.setCheckSlot(readyCount);
    }
    #endregion
    #region statPopup
    public void ShowStatPopup(PlayerInfo playerinfo)
    {
        statUIPopup = PopupManager.Instance.ShowPopup<StatPopup>(statUIPrefab);
        statUIPopup.Init(playerinfo);
    }
    #endregion

    #region GameResultPopup
    public void ShowGameResultPopup(string resultType) 
    {
        gameResultPoup = PopupManager.Instance.ShowPopup<GameResultPoup>(gameResultPopupPrefab);
        gameResultPoup.Init(resultType); 
    }
    #endregion
    
}