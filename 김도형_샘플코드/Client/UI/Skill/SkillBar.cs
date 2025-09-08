using System.Collections.Generic;
using UnityEngine;
using DataModels;

public class SkillBar : MonoBehaviour
{
    [System.Serializable]
    public class SkillIconEntry { public string skill_type; public Sprite icon; }

    [Header("Prefabs/Refs")]
    [SerializeField] private SkillSlot slotPrefab;
    [SerializeField] private Transform container;

    [Header("Icon Mapping")]
    [SerializeField] private List<SkillIconEntry> iconMap = new();

    private ISkillUser _boundUser;
    private readonly List<SkillSlot> _spawned = new();

    public void Bind(ISkillUser user)
    {
        if (user == null)
        {
            Debug.LogWarning("[SkillBar] Bind 실패: user=null");
            return;
        }
        _boundUser = user;
        Rebuild();
    }

    public void Rebuild()
    {
        // 가드
        if (slotPrefab == null)
        {
            Debug.LogError("[SkillBar] Slot Prefab 미할당");
            return;
        }
        if (container == null)
        {
            Debug.LogError("[SkillBar] Container 미할당");
            return;
        }

        // 기존 제거
        foreach (var s in _spawned)
            if (s) Destroy(s.gameObject);
        _spawned.Clear();

        if (_boundUser == null)
        {
            Debug.LogWarning("[SkillBar] Rebuild: 바운드된 유저가 없습니다.");
            return;
        }

        int built = 0;
        foreach (var skill in _boundUser.GetEquippedSkills())
        {
            if (skill == null) continue;

            var slot = Instantiate(slotPrefab, container);
            _spawned.Add(slot);

            var icon = GetIcon(skill.skill_type);
            string keyLabel = skill.default_key;

            slot.Init(_boundUser, skill, icon, keyLabel);
            built++;
        }

        if (built == 0)
            Debug.LogWarning("[SkillBar] Rebuild: 스킬이 0개입니다. (스킬 로드가 아직 안됐을 수 있음)");
    }

    private Sprite GetIcon(string skillType)
    {
        foreach (var e in iconMap)
            if (!string.IsNullOrEmpty(e.skill_type) && e.skill_type == skillType) return e.icon;
        return null;
    }
}
