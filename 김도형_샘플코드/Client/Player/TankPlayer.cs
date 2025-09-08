using System;
using UnityEngine;
using DataModels;
using System.Collections;
using System.Collections.Generic;

public class TankPlayer : BasePlayer, ISkillUser 
{
    private SkillData _taunt;
    private SkillData _dash;

    private KeyCode _tauntKey = KeyCode.None;
    private KeyCode _dashKey  = KeyCode.None;
    
    private Coroutine _dashCo;
    
    private readonly Dictionary<int, float> _lastUseTime = new();
    
    public event Action<int> OnSkillUsed;


    protected override void Start()
    {
        base.Start();

        // 탱커 스킬 로드
        var table = GameDataManager.Instance.GetTable<SkillData>("skill_data");
        if (table != null)
        {
            foreach (var kv in table)
            {
                var s = kv.Value;
                if (s.job != "tank") continue;

                if (s.skill_type == "TAUNT")
                {
                    _taunt = s;
                    if (!InputKeyParser.TryParse(s.default_key, out _tauntKey))
                        Debug.LogWarning($"[TankPlayer] TAUNT 키 해석 실패: '{s.default_key}'");
                }
                else if (s.skill_type == "DASH_ATTACK")
                {
                    _dash = s;
                    if (!InputKeyParser.TryParse(s.default_key, out _dashKey))
                        Debug.LogWarning($"[TankPlayer] DASH 키 해석 실패: '{s.default_key}'");
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!IsMyPlayer || isDead) return;
        if (!_isGrounded) return;

        // 도발
        if (_taunt != null && _tauntKey != KeyCode.None && Input.GetKeyDown(_tauntKey))
        {
            if (ActionLocked || IsOnCooldown(_taunt)) {}
            else
            {
                MarkUsed(_taunt); // 사용 시각 기록
                SendUseSkill(_taunt.id, Vector2.zero);
                StartCoroutine(TauntLockRoutine(_taunt));
                ResetAllSkillTriggers();
                _animator.SetTrigger("SKILL_TAUNT");
            }
        }

        // 돌진
        if (_dash != null && _dashKey != KeyCode.None && Input.GetKeyDown(_dashKey))
        {
            if (ActionLocked || IsOnCooldown(_dash)) {}
            else
            {
                float dirX = _sr != null && _sr.flipX ?  1f : -1f;
                MarkUsed(_dash); // 사용 시각 기록
                SendUseSkill(_dash.id, new Vector2(dirX, 0f));
                ResetAllSkillTriggers();
                _animator.SetTrigger("SKILL_DASH");

                if (_dashCo != null) StopCoroutine(_dashCo);
                _dashCo = StartCoroutine(DashRoutine(dirX, _dash));
            }
        }
    }
    
    private void ResetAllSkillTriggers()
    {
        _animator.ResetTrigger("SKILL_TAUNT");
        _animator.ResetTrigger("SKILL_DASH");
    }
    
    private IEnumerator DashRoutine(float dirX, SkillData s)
    {
        ActionLocked = true;

        float speed    = (s.dash_speed  > 0f) ? s.dash_speed  : 18f;
        float distance = (s.dash_distance> 0f) ? s.dash_distance: 6f;
        float duration = distance / speed;

        // 물리 이동
        Vector2 vel = new Vector2(dirX * speed, _rb.linearVelocity.y);
        float t = 0f;
        while (t < duration)
        {
            // 공중 진입 방지: 지면에서만 유지하도록 수직속도 보존
            _rb.linearVelocity = vel;
            t += Time.deltaTime;
            yield return null;
        }
        // 정지
        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);

        ActionLocked = false;
    }

    private IEnumerator TauntLockRoutine(SkillData s)
    {
        ActionLocked = true;
        float lockTime = Mathf.Max(0.1f, s.cast_time);
        yield return new WaitForSeconds(lockTime);
        ActionLocked = false;
    }
    
    private bool IsOnCooldown(SkillData s)
    {
        if (s == null) return true;
        if (!_lastUseTime.TryGetValue(s.id, out var last)) return false;
        return (Time.time - last) < Mathf.Max(0f, s.cooldown);
    }

    private void MarkUsed(SkillData s)
    {
        _lastUseTime[s.id] = Time.time;
        OnSkillUsed?.Invoke(s.id);
    }
    
    public IEnumerable<SkillData> GetEquippedSkills()
    {
        if (_taunt != null) yield return _taunt;
        if (_dash  != null) yield return _dash;
    }

    public float GetCooldownRemaining(int skillId)
    {
        SkillData s = null;
        if (_taunt != null && _taunt.id == skillId) s = _taunt;
        else if (_dash != null && _dash.id == skillId) s = _dash;

        if (s == null) return 0f;
        if (!_lastUseTime.TryGetValue(skillId, out var last)) return 0f;

        float total = Mathf.Max(0f, s.cooldown);
        float elapsed = Time.time - last;
        return Mathf.Max(0f, total - elapsed);
    }

    public float GetCooldownTotal(int skillId)
    {
        if (_taunt != null && _taunt.id == skillId) return Mathf.Max(0f, _taunt.cooldown);
        if (_dash  != null && _dash.id  == skillId) return Mathf.Max(0f, _dash.cooldown);
        return 0f;
    }
}
