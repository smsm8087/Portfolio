using UnityEngine;
using UnityEngine.UI;

public class PartyMemberIcon : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBg;
    [SerializeField] private Image ultBar;
    [SerializeField] private Image ultBg;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Image mask;
    
    [Header("Revival System UI")]
    [SerializeField] private GameObject deadIndicator;      // 죽음 표시 오브젝트

    private string playerId;
    private string jobType;
    private float currentHealth;
    private float maxHealth;
    private float currentUlt;
    private float maxUlt;
    private bool isDead = false;

    private void Awake()
    {
        Debug.Log($"PartyMemberIcon Awake 시작: {gameObject.name}");
        
        // 프리팹 구조에 맞춰 컴포넌트 자동 찾기
        if (hpBar == null)
        {
            Transform hpTransform = transform.Find("Canvas (Environment)/member/hp/hp");
            if (hpTransform != null)
            {
                hpBar = hpTransform.GetComponent<Image>();
                Debug.Log("HP Bar 찾음!");
            }
            else
            {
                Debug.LogError("HP Bar 못 찾음! 경로 확인 필요");
            }
        }

        if (hpBg == null)
        {
            Transform hpBgTransform = transform.Find("Canvas (Environment)/member/hp/hpBg");
            if (hpBgTransform != null)
            {
                hpBg = hpBgTransform.GetComponent<Image>();
                Debug.Log("HP Bg 찾음!");
            }
            else
            {
                Debug.LogError("HP Bg 못 찾음!");
            }
        }

        if (ultBar == null)
        {
            Transform ultTransform = transform.Find("Canvas (Environment)/member/ult/ult");
            if (ultTransform != null)
            {
                ultBar = ultTransform.GetComponent<Image>();
                Debug.Log("ULT Bar 찾음!");
            }
            else
            {
                Debug.LogError("ULT Bar 못 찾음! 경로 확인 필요");
            }
        }

        if (ultBg == null)
        {
            Transform ultBgTransform = transform.Find("Canvas (Environment)/member/ult/ultBg");
            if (ultBgTransform != null)
            {
                ultBg = ultBgTransform.GetComponent<Image>();
                Debug.Log("ULT Bg 찾음!");
            }
            else
            {
                Debug.LogError("ULT Bg 못 찾음!");
            }
        }

        if (playerIcon == null)
        {
            Transform iconTransform = transform.Find("Canvas (Environment)/member/IconBg/playerImg");
            if (iconTransform != null)
            {
                playerIcon = iconTransform.GetComponent<Image>();
                Debug.Log("Player Icon 찾음!");
            }
            else
            {
                Debug.LogError("Player Icon 못 찾음!");
            }
        }

        if (mask == null)
        {
            Transform maskTransform = transform.Find("Canvas (Environment)/member/IconBg/mask");
            if (maskTransform != null)
            {
                mask = maskTransform.GetComponent<Image>();
                Debug.Log("Mask 찾음!");
            }
            else
            {
                Debug.LogError("Mask 못 찾음!");
            }
        }
    }

    public void Initialize(string id, string job, Sprite icon = null)
    {
        playerId = id;
        jobType = job;

        if (playerIcon == null) return;

        // 항상 jobType 기반으로 로딩
        if (!string.IsNullOrEmpty(jobType))
        {
            string capitalJob = FirstCharToUpper(jobType);
            string spritePath = $"Character/{capitalJob}/PROFILE_{capitalJob}";

            Sprite overrideSprite = Resources.Load<Sprite>(spritePath);
            Debug.Log($"[PartyMemberIcon] Try load sprite: {spritePath} => {(overrideSprite != null ? "Success" : "Fail")}");
            if (overrideSprite != null)
            {
                icon = overrideSprite;
            }
            else
            {
                Debug.LogWarning($"[PartyMemberIcon] Sprite '{spritePath}'을(를) Resources에서 찾지 못함");
                icon = null; 
            }
        }
        else if (icon == null)
        {
            Debug.LogWarning("playerSprite도 없고 jobType도 없음");
            icon = null;
        }

        if (icon != null)
        {
            playerIcon.sprite = icon;
        }

        UpdateHealth(100f, 100f);
        UpdateUlt(0f, 100f);
        SetDeadState(false);
    }
    
    private string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    public void UpdateHealth(float current, float max)
    {
        currentHealth = current;
        maxHealth = max;

        Debug.Log($"UpdateHealth 호출: {current}/{max}");

        if (hpBar != null)
        {
            float healthPercent = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            hpBar.fillAmount = healthPercent;
            Debug.Log($"HP Bar fillAmount 설정: {healthPercent}");
        }
        else
        {
            Debug.LogError("hpBar가 null입니다!");
        }
    }

    public void UpdateUlt(float current, float max)
    {
        currentUlt = current;
        maxUlt = max;

        Debug.Log($"UpdateUlt 호출: {current}/{max}");

        if (ultBar != null)
        {
            float ultPercent = maxUlt > 0 ? currentUlt / maxUlt : 0f;
            ultBar.fillAmount = ultPercent;
            Debug.Log($"ULT Bar fillAmount 설정: {ultPercent}");
        }
        else
        {
            Debug.LogError("ultBar가 null입니다!");
        }
    }

    public void SetStatus(string status)
    {
        switch (status)
        {
            case "dead":
                SetDeadState(true);
                break;
            case "being_revived":
            case "invulnerable":
            case "normal":
            default:
                SetDeadState(false);
                break;
        }
    }
    
    public void SetDeadState(bool dead)
    {
        isDead = dead;
        
        if (deadIndicator != null)
        {
            deadIndicator.SetActive(dead);
        }
    }

    public string GetPlayerId() => playerId;
    public string GetJobType() => jobType;
}