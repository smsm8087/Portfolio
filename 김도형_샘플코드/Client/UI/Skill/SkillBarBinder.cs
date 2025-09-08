using System.Collections;
using UnityEngine;

public class SkillBarBinder : MonoBehaviour
{
    [SerializeField] private float retryInterval = 0.25f; // 재시도 간격
    [SerializeField] private float maxWaitSeconds = 10f;   // 최대 대기 (필요시 늘려도 됨)

    private SkillBar _bar;
    private ISkillUser _bound;

    private void Awake()
    {
        _bar = GetComponent<SkillBar>();
        if (_bar == null)
            Debug.LogError("[SkillBarBinder] SkillBar 컴포넌트가 없습니다.");
    }

    private void Start()
    {
        if (_bar == null) return;
        StartCoroutine(BindWhenReady());
    }

    private IEnumerator BindWhenReady()
    {
        float elapsed = 0f;
        while (elapsed < maxWaitSeconds)
        {
            // 1) 로컬 플레이어 우선 탐색
            ISkillUser su = FindLocalSkillUser();
            if (su != null && HasAnySkill(su))
            {
                _bound = su;
                _bar.Bind(su);
                Debug.Log("[SkillBarBinder] 로컬 ISkillUser 바인드 완료 + 슬롯 생성");
                yield break;
            }

            // 2) 로컬 못 찾으면 아무 ISkillUser라도 (테스트용)
            if (su == null)
            {
                su = FindAnySkillUser();
                if (su != null && HasAnySkill(su))
                {
                    _bound = su;
                    _bar.Bind(su);
                    Debug.Log("[SkillBarBinder] (Fallback) 첫 ISkillUser 바인드 완료");
                    yield break;
                }
            }

            // 계속 대기/재시도
            elapsed += retryInterval;
            yield return new WaitForSeconds(retryInterval);
        }

        Debug.LogWarning("[SkillBarBinder] 바인드 실패: 로컬 플레이어 혹은 스킬 목록을 찾지 못했습니다.");
    }

    private ISkillUser FindLocalSkillUser()
    {
        var all = FindObjectsOfType<MonoBehaviour>();
        foreach (var mb in all)
        {
            if (mb is ISkillUser su)
            {
                var bp = mb.GetComponent<BasePlayer>();
                if (bp != null && bp.IsMyPlayer) // 로컬 플레이어만
                    return su;
            }
        }
        return null;
    }

    private ISkillUser FindAnySkillUser()
    {
        var all = FindObjectsOfType<MonoBehaviour>();
        foreach (var mb in all)
            if (mb is ISkillUser su) return su;
        return null;
    }

    private bool HasAnySkill(ISkillUser su)
    {
        // GetEquippedSkills()에 최소 1개라도 있으면 true
        foreach (var _ in su.GetEquippedSkills())
            return true;
        return false;
    }
}
