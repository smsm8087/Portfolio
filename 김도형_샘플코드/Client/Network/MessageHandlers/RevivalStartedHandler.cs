using UnityEngine;

public class RevivalStartedHandler : INetworkMessageHandler
{
    public string Type => "revival_started";
    
    public void Handle(NetMsg msg)
    {
        Debug.Log($"[RevivalStartedHandler] 메시지 받음!");
        Debug.Log($"[RevivalStartedHandler] targetId={msg.targetId}, reviverId={msg.reviverId}");
        Debug.Log($"[RevivalStartedHandler] duration={msg.duration}");
    
        // RevivalUI.Instance 확인
        if (RevivalUI.Instance != null)
        {
            Debug.Log("[RevivalStartedHandler] RevivalUI.Instance 존재함 - ShowRevivalProgress 호출");
            RevivalUI.Instance.ShowRevivalProgress(msg.targetId, msg.reviverId, msg.duration);
        }
        else
        {
            Debug.LogError("[RevivalStartedHandler] RevivalUI.Instance가 null!");
        }
        
        // 부활 대상 플레이어 상태 업데이트
        var players = NetworkManager.Instance.GetPlayers();
        if (players.TryGetValue(msg.targetId, out GameObject targetPlayerObj))
        {
            BasePlayer targetPlayer = targetPlayerObj.GetComponent<BasePlayer>();
            if (targetPlayer != null)
            {
                targetPlayer.SetRevivalState(true, msg.reviverId);
                Debug.Log($"[RevivalStartedHandler] {msg.targetId} 플레이어 상태 업데이트 완료");
            }
        }
        else
        {
            Debug.LogError($"[RevivalStartedHandler] 플레이어 {msg.targetId}를 찾을 수 없음");
        }
        
        Debug.Log($"[RevivalStartedHandler] 처리 완료");
    }
}