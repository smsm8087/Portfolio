using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataModels;

public class SkillSlot : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private ISkillUser _user;
    private SkillData _skill;
    private float _lastShownRemain;

    /// <summary>아이콘/키/타겟 스킬 바인딩</summary>
    public void Init(ISkillUser user, SkillData skill, Sprite iconSprite, string keyLabel)
    {
        _user = user;
        _skill = skill;

        if (icon) icon.sprite = iconSprite;
        if (keyText) keyText.text = keyLabel?.ToUpperInvariant();

        // 초기 쿨 표현
        UpdateVisual(force: true);

        // 스킬 사용시 즉시 갱신
        if (_user != null)
            _user.OnSkillUsed += HandleSkillUsed;
    }

    private void OnDestroy()
    {
        if (_user != null)
            _user.OnSkillUsed -= HandleSkillUsed;
    }

    private void HandleSkillUsed(int skillId)
    {
        if (_skill != null && _skill.id == skillId)
            UpdateVisual(force: true);
    }

    private void Update()
    {
        UpdateVisual(force: false);
    }

    private void UpdateVisual(bool force)
    {
        if (_user == null || _skill == null) return;

        float remain = Mathf.Max(0f, _user.GetCooldownRemaining(_skill.id));
        float total  = Mathf.Max(0f, _user.GetCooldownTotal(_skill.id));
        float ratio  = (total <= 0.0001f) ? 0f : (remain / total);

        // Fill (0=없음, 1=풀 쿨다운)
        if (cooldownFill)
            cooldownFill.fillAmount = ratio;

        // 숫자 표기
        if (cooldownText)
        {
            if (remain > 0.05f)
            {
                int sec = Mathf.CeilToInt(remain);
                if (force || Mathf.Abs(_lastShownRemain - sec) >= 1f)
                {
                    cooldownText.text = sec.ToString();
                    _lastShownRemain = sec;
                }
                cooldownText.enabled = true;
            }
            else
            {
                cooldownText.enabled = false;
            }
        }

        // 쿨타임 중 아이콘 그레이 효과
        if (icon)
            icon.color = (remain > 0f) ? new Color(1f,1f,1f,0.5f) : Color.white;
    }
}
