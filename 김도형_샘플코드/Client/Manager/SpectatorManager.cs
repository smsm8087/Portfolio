using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// 사망한 플레이어가 다른 살아있는 플레이어들을 관전할 수 있도록 하는 시스템
/// </summary>
public class SpectatorManager : MonoBehaviour
{
    public static SpectatorManager Instance;
    [Header("Spectator Settings")]
    public float switchCooldown = 1f;
    
    private bool isSpectating = false;
    private int currentSpectatorIndex = 0;
    private List<string> alivePlayerIds = new List<string>();
    private float lastSwitchTime = 0f;
    
    [Header("UI References")]
    public GameObject spectatorUI;
    public TextMeshProUGUI spectatorInfoText;
    public UnityEngine.UI.Button nextPlayerButton;
    public UnityEngine.UI.Button prevPlayerButton;
    
    private Dictionary<string, GameObject> players;
    private CameraFollow cameraFollow;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // UI 버튼 이벤트 연결
        if (nextPlayerButton != null)
            nextPlayerButton.onClick.AddListener(NextPlayer);
        if (prevPlayerButton != null)
            prevPlayerButton.onClick.AddListener(PrevPlayer);
    }
    
    void Start()
    {
        // NetworkManager와 플레이어 딕셔너리 null 체크
        if (NetworkManager.Instance == null)
        {
            Debug.LogError("[SpectatorManager] NetworkManager.Instance가 null입니다.");
            return;
        }
        
        // NetworkManager에서 플레이어 딕셔너리 가져오기
        players = NetworkManager.Instance.GetPlayers();
        
        if (players == null)
        {
            Debug.LogError("[SpectatorManager] players 딕셔너리가 null입니다.");
            return;
        }
        
        // 초기에는 관전 UI 비활성화
        SetSpectatorUI(false);
        
        Debug.Log("[SpectatorManager] 초기화 완료");
    }
    
    /// <summary>
    /// CameraFollow 안전하게 가져오기
    /// </summary>
    private CameraFollow GetCameraFollow()
    {
        if (cameraFollow == null)
        {
            cameraFollow = CameraFollow.Instance;
        }
        
        if (cameraFollow == null)
        { 
            cameraFollow = FindFirstObjectByType<CameraFollow>();
        }
        
        return cameraFollow;
    }
    
    void Update()
    {
        if (!isSpectating) return;
        
        // 키보드 입력으로 플레이어 전환 (PC용)
        if (Input.GetKeyDown(KeyCode.Q) && Time.time - lastSwitchTime > switchCooldown)
        {
            PrevPlayer();
        }
        else if (Input.GetKeyDown(KeyCode.E) && Time.time - lastSwitchTime > switchCooldown)
        {
            NextPlayer();
        }
    }
    
    /// <summary>
    /// 관전 모드 시작
    /// </summary>
    public void StartSpectating()
    {
        if (isSpectating) return;
        
        isSpectating = true;
        UpdateAlivePlayersList();
        
        if (MobileInputUI.Instance != null)
        {
            MobileInputUI.Instance.SetInputEnabled(false);
        }
        
        if (alivePlayerIds.Count > 0)
        {
            currentSpectatorIndex = 0;
            SwitchToPlayer(alivePlayerIds[currentSpectatorIndex]);
            SetSpectatorUI(true);
        }
        else
        {
            // 살아있는 플레이어가 없으면 게임 종료 처리
            Debug.Log("[SpectatorSystem] 살아있는 플레이어가 없습니다.");
            SetSpectatorUI(false);
        }
    }
    
    /// <summary>
    /// 관전 모드 종료
    /// </summary>
    public void StopSpectating()
    {
        if (!isSpectating) return;
        
        isSpectating = false;
        SetSpectatorUI(false);
        
        if (MobileInputUI.Instance != null)
        {
            MobileInputUI.Instance.SetInputEnabled(true);
        }
        
        // 자신의 캐릭터로 카메라 복귀
        if (players != null && players.TryGetValue(NetworkManager.Instance.MyUserId, out GameObject myPlayer) && myPlayer != null)
        {
            CameraFollow camera = GetCameraFollow();
            if (camera != null)
            {
                camera.setTarget(myPlayer.transform);
            }
        }
    }
    
    /// <summary>
    /// 다음 플레이어로 전환
    /// </summary>
    public void NextPlayer()
    {
        if (!isSpectating || Time.time - lastSwitchTime < switchCooldown) return;
        
        UpdateAlivePlayersList();
        
        if (alivePlayerIds.Count == 0)
        {
            StopSpectating();
            return;
        }
        
        currentSpectatorIndex = (currentSpectatorIndex + 1) % alivePlayerIds.Count;
        SwitchToPlayer(alivePlayerIds[currentSpectatorIndex]);
        lastSwitchTime = Time.time;
    }
    
    /// <summary>
    /// 이전 플레이어로 전환
    /// </summary>
    public void PrevPlayer()
    {
        if (!isSpectating || Time.time - lastSwitchTime < switchCooldown) return;
        
        UpdateAlivePlayersList();
        
        if (alivePlayerIds.Count == 0)
        {
            StopSpectating();
            return;
        }
        
        currentSpectatorIndex = (currentSpectatorIndex - 1 + alivePlayerIds.Count) % alivePlayerIds.Count;
        SwitchToPlayer(alivePlayerIds[currentSpectatorIndex]);
        lastSwitchTime = Time.time;
    }
    
    /// <summary>
    /// 살아있는 플레이어 목록 업데이트
    /// </summary>
    private void UpdateAlivePlayersList()
    {
        alivePlayerIds.Clear();
        
        foreach (var kvp in players)
        {
            if (kvp.Value != null)
            {
                BasePlayer player = kvp.Value.GetComponent<BasePlayer>();
                if (player != null && !player.isDead && kvp.Key != NetworkManager.Instance.MyUserId)
                {
                    alivePlayerIds.Add(kvp.Key);
                }
            }
        }
    }
    
    /// <summary>
    /// 특정 플레이어로 카메라 전환
    /// </summary>
    private void SwitchToPlayer(string playerId)
    {
        // null 체크 추가
        if (players == null)
        {
            Debug.LogError("[SpectatorManager] players 딕셔너리가 null입니다.");
            return;
        }
        
        CameraFollow camera = GetCameraFollow();
        if (camera == null)
        {
            Debug.LogError("[SpectatorManager] CameraFollow를 찾을 수 없습니다.");
            return;
        }
        
        if (players.TryGetValue(playerId, out GameObject playerObj) && playerObj != null)
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                camera.setTarget(playerObj.transform);
                UpdateSpectatorUI(player);
                Debug.Log($"[SpectatorManager] {playerId} 플레이어로 카메라 전환");
            }
            else
            {
                Debug.LogError($"[SpectatorManager] {playerId} 플레이어에 BasePlayer 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"[SpectatorManager] {playerId} 플레이어를 찾을 수 없습니다.");
        }
    }
    
    /// <summary>
    /// 관전 UI 활성화/비활성화
    /// </summary>
    private void SetSpectatorUI(bool active)
    {
        if (spectatorUI != null)
        {
            spectatorUI.SetActive(active);
        }
    }
    
    /// <summary>
    /// 관전 UI 정보 업데이트
    /// </summary>
    private void UpdateSpectatorUI(BasePlayer targetPlayer)
    {
        if (spectatorInfoText != null && targetPlayer != null)
        {
            spectatorInfoText.text = $"닉네임은여덟글자\n<size=50%>관전중</size>";
        }
    }
    
    /// <summary>
    /// 플레이어가 사망했을 때 호출 (외부에서 호출)
    /// </summary>
    public void OnPlayerDied(string playerId)
    {
        // 관전 중인 플레이어가 죽었다면 다른 플레이어로 전환
        if (isSpectating && alivePlayerIds.Contains(playerId))
        {
            UpdateAlivePlayersList();
            
            if (alivePlayerIds.Count == 0)
            {
                StopSpectating();
            }
            else
            {
                // 현재 인덱스 조정
                if (currentSpectatorIndex >= alivePlayerIds.Count)
                {
                    currentSpectatorIndex = 0;
                }
                SwitchToPlayer(alivePlayerIds[currentSpectatorIndex]);
            }
        }
    }
}
