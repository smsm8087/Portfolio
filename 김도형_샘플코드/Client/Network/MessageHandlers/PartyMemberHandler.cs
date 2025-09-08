using System.Collections.Generic;
using UnityEngine;
using DataModels;

// 파티 멤버 체력 업데이트 핸들러
public class PartyMemberHealthHandler : INetworkMessageHandler
{
    public string Type => "party_member_health";

    public void Handle(NetMsg msg)
    {
        if (PartyMemberUI.Instance == null)
        {
            Debug.LogWarning("PartyMemberUI 인스턴스가 없습니다.");
            return;
        }

        string playerId = msg.player_id;
        float currentHealth = msg.current_health;
        float maxHealth = msg.max_health;

        PartyMemberUI.Instance.UpdateMemberHealth(playerId, currentHealth, maxHealth);
        
        Debug.Log($"[PartyMemberHealthHandler] {playerId} 체력 업데이트: {currentHealth}/{maxHealth}");
    }
}

// 파티 멤버 궁극기 업데이트 핸들러
public class PartyMemberUltHandler : INetworkMessageHandler
{
    public string Type => "party_member_ult";

    public void Handle(NetMsg msg)
    {
        if (PartyMemberUI.Instance == null)
        {
            Debug.LogWarning("PartyMemberUI 인스턴스가 없습니다.");
            return;
        }

        string playerId = msg.player_id;
        float currentUlt = msg.current_ult;
        float maxUlt = msg.max_ult;

        PartyMemberUI.Instance.UpdateMemberUlt(playerId, currentUlt, maxUlt);
        
        Debug.Log($"[PartyMemberUltHandler] {playerId} 궁극기 업데이트: {currentUlt}/{maxUlt}");
    }
}

// 파티 멤버 상태 업데이트 핸들러
public class PartyMemberStatusHandler : INetworkMessageHandler
{
    public string Type => "party_member_status";

    public void Handle(NetMsg msg)
    {
        if (PartyMemberUI.Instance == null)
        {
            Debug.LogWarning("PartyMemberUI 인스턴스가 없습니다.");
            return;
        }

        string playerId = msg.player_id;
        string status = msg.status;

        PartyMemberUI.Instance.UpdateMemberStatus(playerId, status);
        
        Debug.Log($"[PartyMemberStatusHandler] {playerId} 상태 업데이트: {status}");
    }
}

// 파티 정보 전체 업데이트 핸들러
public class PartyInfoHandler : INetworkMessageHandler
{
    public string Type => "party_info";

    public void Handle(NetMsg msg)
    {
        if (PartyMemberUI.Instance == null)
        {
            Debug.LogWarning("PartyMemberUI 인스턴스가 없습니다.");
            return;
        }

        if (msg.members == null)
        {
            Debug.LogWarning("파티 멤버 정보가 null입니다.");
            return;
        }

        List<PartyMemberData> partyMembers = new List<PartyMemberData>();
        
        foreach (var member in msg.members)
        {
            partyMembers.Add(new PartyMemberData
            {
                id = member.id,
                job_type = member.job_type,
                current_health = member.current_health,
                max_health = member.max_health,
                current_ult = member.current_ult,
                max_ult = member.max_ult
            });
        }

        PartyMemberUI.Instance.UpdatePartyInfo(partyMembers);
        
        Debug.Log($"[PartyInfoHandler] 파티 정보 업데이트 완료. 멤버 수: {partyMembers.Count}");
    }
}

// 파티 멤버 퇴장 핸들러
public class PartyMemberLeftHandler : INetworkMessageHandler
{
    public string Type => "party_member_left";

    public void Handle(NetMsg msg)
    {
        if (PartyMemberUI.Instance == null)
        {
            Debug.LogWarning("PartyMemberUI 인스턴스가 없습니다.");
            return;
        }

        string playerId = msg.player_id;
        PartyMemberUI.Instance.RemovePartyMember(playerId);
        
        Debug.Log($"[PartyMemberLeftHandler] {playerId} 파티에서 퇴장");
    }
}

// 통합 파티 멤버 핸들러
public class PartyMemberHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, INetworkMessageHandler> handlers;

    public string Type => "party_member";

    public PartyMemberHandler()
    {
        handlers = new Dictionary<string, INetworkMessageHandler>
        {
            {"party_member_health", new PartyMemberHealthHandler()},
            {"party_member_ult", new PartyMemberUltHandler()},
            {"party_member_status", new PartyMemberStatusHandler()},
            {"party_info", new PartyInfoHandler()},
            {"party_member_left", new PartyMemberLeftHandler()}
        };
    }

    public void Handle(NetMsg msg)
    {
        if (handlers.TryGetValue(msg.type, out INetworkMessageHandler handler))
        {
            handler.Handle(msg);
        }
        else
        {
            Debug.LogWarning($"알 수 없는 파티 메시지 타입: {msg.type}");
        }
    }
}