using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 부활 시스템 UI 관리 클래스
/// 부활 게이지, 상호작용 버튼, 상태 표시 등을 담당
/// </summary>
public class RevivalUI : MonoBehaviour
{
    public static RevivalUI Instance { get; private set; }

    [Header("Revival Gauge")]
    [SerializeField] private GameObject revivalGaugePanel;
    [SerializeField] private Image revivalGaugeCircle;
    [SerializeField] private TextMeshProUGUI revivalText;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Interaction UI")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private Button revivalButton;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Settings")]
    [SerializeField] private float uiShowDistance = 3.0f;

    private Dictionary<string, RevivalData> activeRevivals = new Dictionary<string, RevivalData>();
    private string currentRevivalTarget = "";
    private bool isShowingInteraction = false;

    private class RevivalData
    {
        public string targetId;
        public string reviverId;
        public float duration;
        public float progress;
        public Vector3 position;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[RevivalUI] RevivalUI 인스턴스 생성됨");
    }

    private void Start()
    {
        // 초기 상태 설정
        HideAllPanels();
        
        // 버튼 이벤트 연결
        if (revivalButton != null)
        {
            revivalButton.onClick.AddListener(OnRevivalButtonPressed);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[TEST] T키로 강제 부활 UI 표시");
            ShowRevivalProgress("test_target", "test_reviver", 3.0f);
        }
        
        UpdateInteractionUI();
    }

    /// <summary>
    /// 모든 패널 숨기기
    /// </summary>
    private void HideAllPanels()
    {
        if (revivalGaugePanel) revivalGaugePanel.SetActive(false);
        if (interactionPanel) interactionPanel.SetActive(false);
    }

    /// <summary>
    /// 상호작용 UI 업데이트
    /// </summary>
    private void UpdateInteractionUI()
    {
        if (NetworkManager.Instance?.MyUserId == null) return;

        var players = NetworkManager.Instance.GetPlayers();
        if (!players.TryGetValue(NetworkManager.Instance.MyUserId, out GameObject myPlayerObj))
            return;

        BasePlayer myPlayer = myPlayerObj.GetComponent<BasePlayer>();
        if (myPlayer == null || myPlayer.isDead) return;

        // 근처 부활 가능한 플레이어 찾기
        string nearbyDeadPlayer = FindNearbyRevivablePlayer(myPlayer);
        
        if (!string.IsNullOrEmpty(nearbyDeadPlayer))
        {
            ShowInteractionUI(nearbyDeadPlayer);
        }
        else
        {
            HideInteractionUI();
        }
    }

    /// <summary>
    /// 부활 가능한 근처 플레이어 찾기
    /// </summary>
    private string FindNearbyRevivablePlayer(BasePlayer myPlayer)
    {
        var players = NetworkManager.Instance.GetPlayers();
        
        foreach (var kvp in players)
        {
            if (kvp.Key == NetworkManager.Instance.MyUserId) continue;
            
            GameObject playerObj = kvp.Value;
            if (playerObj == null) continue;
            
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player == null || !player.isDead || player.isBeingRevived) continue;
            
            float distance = Vector3.Distance(myPlayer.transform.position, player.deathPosition);
            if (distance <= uiShowDistance)
            {
                return kvp.Key;
            }
        }
        
        return null;
    }

    /// <summary>
    /// 상호작용 UI 표시
    /// </summary>
    private void ShowInteractionUI(string targetId)
    {
        // 이미 같은 대상에 대해 표시 중이면 중복 호출 방지
        if (isShowingInteraction && currentRevivalTarget == targetId) return;
    
        currentRevivalTarget = targetId;
        isShowingInteraction = true;
    
        Debug.Log($"[RevivalUI] 상호작용 UI 표시: {targetId}"); // 로그 추가
    
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
        
            if (interactionText != null)
            {
                interactionText.text = $"F키를 눌러 {GetPlayerJobType(targetId)} 부활";
            }
        }
    }

    /// <summary>
    /// 상호작용 UI 숨기기
    /// </summary>
    private void HideInteractionUI()
    {
        if (!isShowingInteraction) return;
        
        isShowingInteraction = false;
        currentRevivalTarget = "";
        
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 부활 버튼 클릭 이벤트
    /// </summary>
    private void OnRevivalButtonPressed()
    {
        if (string.IsNullOrEmpty(currentRevivalTarget)) return;
        
        // 부활 시작 메시지 전송
        var revivalMsg = new NetMsg
        {
            type = "start_revival",
            playerId = NetworkManager.Instance.MyUserId,
            targetId = currentRevivalTarget
        };
        NetworkManager.Instance.SendMsg(revivalMsg);
    }

    /// <summary>
    /// 부활 진행 상황 표시 시작
    /// </summary>
    public void ShowRevivalProgress(string targetId, string reviverId, float duration)
    {
        Debug.Log($"[RevivalUI] ShowRevivalProgress 시작");
        Debug.Log($"[RevivalUI] targetId: {targetId}, reviverId: {reviverId}, duration: {duration}");
        Debug.Log($"[RevivalUI] activeRevivals에 이미 있는지: {activeRevivals.ContainsKey(targetId)}");
    
        // 이미 진행 중인 부활이면 중복 방지
        if (activeRevivals.ContainsKey(targetId))
        {
            Debug.Log($"[RevivalUI] 이미 진행 중인 부활: {targetId}");
            return;
        }
    
        Debug.Log($"[RevivalUI] 새로운 부활 데이터 생성");
    
        var revivalData = new RevivalData
        {
            targetId = targetId,
            reviverId = reviverId,
            duration = duration,
            progress = 0f
        };
    
        activeRevivals[targetId] = revivalData;
        Debug.Log($"[RevivalUI] activeRevivals에 추가 완료");
    
        // UI 업데이트
        Debug.Log($"[RevivalUI] revivalGaugePanel null 체크: {revivalGaugePanel == null}");
        if (revivalGaugePanel != null)
        {
            Debug.Log($"[RevivalUI] Panel 활성화 전 상태: {revivalGaugePanel.activeSelf}");
            revivalGaugePanel.SetActive(true);
            Debug.Log($"[RevivalUI] Panel 활성화 후 상태: {revivalGaugePanel.activeSelf}");
        
            // 부모 오브젝트들도 활성화되어 있는지 확인
            Transform parent = revivalGaugePanel.transform.parent;
            while (parent != null)
            {
                Debug.Log($"[RevivalUI] 부모 '{parent.name}' 활성화 상태: {parent.gameObject.activeSelf}");
                parent = parent.parent;
            }
        }
        else
        {
            Debug.LogError("[RevivalUI] revivalGaugePanel이 null!");
        }
    
        Debug.Log($"[RevivalUI] ShowRevivalProgress 완료");
    
        // 상호작용 UI 숨기기
        HideInteractionUI();
    }

    /// <summary>
    /// 부활 진행률 업데이트
    /// </summary>
    public void UpdateRevivalProgress(string targetId, float progress)
    {
        if (!activeRevivals.TryGetValue(targetId, out RevivalData data)) return;
        
        data.progress = progress;
        
        // 원형 게이지 업데이트
        if (revivalGaugeCircle != null)
        {
            revivalGaugeCircle.fillAmount = progress / 100f;
        }
        
        // 진행률 텍스트 업데이트
        if (progressText != null)
        {
            progressText.text = $"{progress:F0}%";
        }
    }

    /// <summary>
    /// 부활 완료 처리
    /// </summary>
    public void OnRevivalCompleted(string targetId)
    {
        if (activeRevivals.ContainsKey(targetId))
        {
            activeRevivals.Remove(targetId);
        }
        
        // UI 숨기기
        if (revivalGaugePanel != null)
        {
            revivalGaugePanel.SetActive(false);
        }
        
        // 완료 메시지 표시
        ShowRevivalMessage($"{GetPlayerJobType(targetId)}이(가) 부활했습니다!", 2f);
    }

    /// <summary>
    /// 부활 취소 처리
    /// </summary>
    public void OnRevivalCancelled(string targetId, string reason)
    {
        if (activeRevivals.ContainsKey(targetId))
        {
            activeRevivals.Remove(targetId);
        }
        
        // UI 숨기기
        if (revivalGaugePanel != null)
        {
            revivalGaugePanel.SetActive(false);
        }
        
        // 취소 이유에 따른 메시지
        string message = reason switch
        {
            "hit_during_revival" => "피격으로 인해 부활이 중단되었습니다!",
            "reviver_died" => "부활시키는 플레이어가 사망했습니다!",
            "player_cancelled" => "부활이 취소되었습니다.",
            _ => "부활이 중단되었습니다."
        };
        
        ShowRevivalMessage(message, 2f);
    }

    /// <summary>
    /// 일시적 메시지 표시
    /// </summary>
    public void ShowRevivalMessage(string message, float duration)
    {
        StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    /// <summary>
    /// 메시지 표시 코루틴
    /// </summary>
    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        // 임시 메시지 UI 생성 또는 기존 UI 활용
        Debug.Log($"[RevivalUI] {message}");
        
        // TODO: 실제 UI 메시지 표시 로직 구현
        yield return new WaitForSeconds(duration);
    }

    /// <summary>
    /// 플레이어 직업 타입 가져오기
    /// </summary>
    private string GetPlayerJobType(string playerId)
    {
        var players = NetworkManager.Instance.GetPlayers();
        if (players.TryGetValue(playerId, out GameObject playerObj))
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                return player.job_type;
            }
        }
        return "플레이어";
    }
}