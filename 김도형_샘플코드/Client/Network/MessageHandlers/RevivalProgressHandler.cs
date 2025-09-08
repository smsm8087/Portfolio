using UnityEngine;

public class RevivalProgressHandler : INetworkMessageHandler
{
    public string Type => "revival_progress";
    
    public void Handle(NetMsg msg)
    {
        // 부활 게이지 업데이트
        if (RevivalUI.Instance != null)
        {
            RevivalUI.Instance.UpdateRevivalProgress(msg.targetId, msg.progress);
        }
    }
}