using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerListHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> prefabMap;
    private readonly Dictionary<string, GameObject> players;

    public string Type => "player_list";
    private List<PlayerInfo> playerList;

    public PlayerListHandler(Dictionary<string, GameObject> prefabMap, Dictionary<string, GameObject> players)
    {
        this.prefabMap = prefabMap;
        this.players = players;
    }
    public void Handle(NetMsg msg)
    {
        if (msg.players == null) return;
        playerList = msg.players;
        Debug.Log($"[PlayerListHandler] 플레이어 목록 처리 시작. 총 {msg.players.Count}명");
        foreach (var playerData in playerList)
        {
            string pid = playerData.id; // .ToString() 제거
            string jobType = playerData.job_type;
            
            Debug.Log($"[PlayerListHandler] 처리 중: PID={pid}, JobType={jobType}, MyGUID={NetworkManager.Instance.MyUserId}");
            
            // 잘못된 데이터 필터링
            if (string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(jobType))
            {
                Debug.LogWarning($"[PlayerListHandler] 잘못된 플레이어 데이터 스킵: PID={pid}, JobType={jobType}");
                continue;
            }
            
            // 이미 존재하는 플레이어는 스킵
            if (players.ContainsKey(pid))
            {
                Debug.Log($"[PlayerListHandler] 이미 존재하는 플레이어 스킵: {pid}");
                continue;
            }
            
            var playerObj = GameObject.Instantiate(prefabMap[jobType]);
            players[pid] = playerObj;
            
            NetworkCharacterFollower playerFollower = playerObj.GetComponent<NetworkCharacterFollower>();
            if (playerFollower)
            {
                playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
            }
            
            Debug.Log($"[PlayerListHandler] 다른 플레이어 생성 완료: {pid}");
        }
    }
}