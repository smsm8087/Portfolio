using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnClickMainMenu);
        backButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
       
    }

    public void UpdateGoldUI(int gold)
    {
        goldText.text = gold.ToString();
    }

    public void SetBackButtonListener(UnityEngine.Events.UnityAction action)
    {
        backButton.onClick.RemoveAllListeners();

        if (action != null)
        {
            backButton.onClick.AddListener(action);
            backButton.gameObject.SetActive(true);
        }
        else
        {
            backButton.gameObject.SetActive(false);
        }
    }

    private void OnClickMainMenu()
    {
    }
}