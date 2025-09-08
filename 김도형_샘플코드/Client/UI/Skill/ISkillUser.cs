using System;
using System.Collections.Generic;
using DataModels;

public interface ISkillUser
{
    /// <summary>장착/표시할 스킬들 반환</summary>
    IEnumerable<SkillData> GetEquippedSkills();

    /// <summary>해당 스킬의 남은 쿨타임(초). 없으면 0</summary>
    float GetCooldownRemaining(int skillId);

    /// <summary>해당 스킬의 총 쿨타임(초). 없으면 0</summary>
    float GetCooldownTotal(int skillId);

    /// <summary>스킬 사용 시 호출되는 이벤트(스킬ID)</summary>
    event Action<int> OnSkillUsed;
}