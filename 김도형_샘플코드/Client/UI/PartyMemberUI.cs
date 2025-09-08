using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataModels;

public class PartyMemberData
{
    public string id { get; set; }
    public string job_type { get; set; }
    public float current_health { get; set; }
    public float max_health { get; set; }
    public float current_ult { get; set; }
    public float max_ult { get; set; }
    public bool is_dead { get; set; }
    public bool is_being_revived { get; set; }
    public bool is_invulnerable { get; set; }
    public string revived_by { get; set; }
    public float? death_position_x { get; set; }
    public float? death_position_y { get; set; }
}

public class PartyMemberUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform partyContainer;
    [SerializeField] private GameObject memberPrefab;

    [Header("Job Icons")]
    [SerializeField] private Sprite tankIcon;
    [SerializeField] private Sprite programmerIcon;
    [SerializeField] private Sprite sniperIcon;

    private Dictionary<string, PartyMemberIcon> partyMembers = new Dictionary<string, PartyMemberIcon>();
    private Dictionary<string, Sprite> jobIcons = new Dictionary<string, Sprite>();

    public static PartyMemberUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 자동으로 컨테이너 찾기
        if (partyContainer == null)
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj != null)
            {
                Transform partyMemberTransform = canvasObj.transform.Find("PartyMember");
                if (partyMemberTransform != null)
                    partyContainer = partyMemberTransform;
            }
        }

        InitializeJobIcons();
    }

    private void InitializeJobIcons()
    {
        if (tankIcon != null)
            jobIcons["tank"] = tankIcon;
        if (programmerIcon != null)
            jobIcons["programmer"] = programmerIcon;
        if (sniperIcon != null)
            jobIcons["sniper"] = sniperIcon;
    }

    public void AddPartyMember(string playerId, string jobType)
    {
        // 내 플레이어는 파티 UI에 추가하지 않음
        if (playerId == NetworkManager.Instance.MyUserId)
        {
            Debug.Log($"내 플레이어({playerId})는 파티 UI에서 제외");
            return;
        }

        if (partyMembers.ContainsKey(playerId))
        {
            Debug.LogWarning($"파티원 {playerId}가 이미 존재합니다.");
            return;
        }

        if (memberPrefab == null || partyContainer == null)
        {
            Debug.LogError("멤버 프리팹 또는 파티 컨테이너가 설정되지 않았습니다.");
            return;
        }

        // 프리팹 생성
        GameObject memberObj = Instantiate(memberPrefab, partyContainer);
        PartyMemberIcon memberIcon = memberObj.GetComponent<PartyMemberIcon>();

        if (memberIcon == null)
        {
            memberIcon = memberObj.AddComponent<PartyMemberIcon>();
        }

        // 직업에 맞는 아이콘 설정
        Sprite icon = jobIcons.ContainsKey(jobType) ? jobIcons[jobType] : null;
        memberIcon.Initialize(playerId, jobType, icon);

        partyMembers[playerId] = memberIcon;

        Debug.Log($"파티원 UI 추가: {playerId} ({jobType})");
    }

    public void RemovePartyMember(string playerId)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            if (memberIcon != null)
                Destroy(memberIcon.gameObject);
            
            partyMembers.Remove(playerId);
            Debug.Log($"파티원 UI 제거: {playerId}");
        }
    }

    public void UpdateMemberHealth(string playerId, float currentHp, float maxHp)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            memberIcon.UpdateHealth(currentHp, maxHp);
        }
    }

    public void UpdateMemberUlt(string playerId, float currentUlt, float maxUlt)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            memberIcon.UpdateUlt(currentUlt, maxUlt);
        }
    }

    public void UpdateMemberStatus(string playerId, string status)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            memberIcon.SetStatus(status);
        }
    }

    public void UpdatePartyInfo(List<PartyMemberData> members)
    {
        // 내 플레이어 제외한 멤버들만 필터링
        var otherMembers = members.Where(m => m.id != NetworkManager.Instance.MyUserId).ToList();
        
        // 기존 멤버들과 비교하여 추가/제거
        var currentMemberIds = partyMembers.Keys.ToHashSet();
        var newMemberIds = otherMembers.Select(m => m.id).ToHashSet();

        // 제거할 멤버들
        var toRemove = currentMemberIds.Except(newMemberIds);
        foreach (var memberId in toRemove)
        {
            RemovePartyMember(memberId);
        }

        // 추가/업데이트할 멤버들
        foreach (var member in otherMembers)
        {
            if (!partyMembers.ContainsKey(member.id))
            {
                AddPartyMember(member.id, member.job_type);
            }

            // 정보 업데이트
            UpdateMemberHealth(member.id, member.current_health, member.max_health);
            UpdateMemberUlt(member.id, member.current_ult, member.max_ult);
            
            // 상태 업데이트
            string status = "normal";
            if (member.is_dead)
                status = "dead";
            else if (member.is_being_revived)
                status = "being_revived";
            else if (member.is_invulnerable)
                status = "invulnerable";
                
            UpdateMemberStatus(member.id, status);
        }
    }
    
    public void ClearAllMembers()
    {
        foreach (var kvp in partyMembers)
        {
            if (kvp.Value != null)
                Destroy(kvp.Value.gameObject);
        }
        partyMembers.Clear();
    }

    public int GetMemberCount()
    {
        return partyMembers.Count;
    }
}