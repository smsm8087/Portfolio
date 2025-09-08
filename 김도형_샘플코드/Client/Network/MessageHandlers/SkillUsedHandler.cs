using UnityEngine;
using DataModels;
using System.Collections.Generic;

public class SkillUsedHandler : INetworkMessageHandler
{
    public string Type => "skill_used";

    public void Handle(NetMsg msg)
    {
        // 스킬 데이터 가져오기
        var skill = GameDataManager.Instance.GetData<SkillData>("skill_data", msg.skillId);
        if (skill == null) return;

        // 스킬 사용한 플레이어 찾기
        var players = NetworkManager.Instance.GetPlayers();
        if (!players.TryGetValue(msg.playerId, out var playerGO) || playerGO == null) return;

        var animator = playerGO.GetComponent<Animator>();
        if (animator == null) return;

        // 플레이어 애니메이션 재생
        switch (skill.skill_type)
        {
            case "TAUNT":
                // 도발 애니메이션 트리거
                animator.ResetTrigger("SKILL_DASH");
                animator.SetTrigger("SKILL_TAUNT");
                break;

            case "DASH_ATTACK":
                // 돌진 애니메이션 트리거
                animator.ResetTrigger("SKILL_TAUNT");
                animator.SetTrigger("SKILL_DASH");
                break;
        }
    }
}