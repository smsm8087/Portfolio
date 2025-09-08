using UnityEngine;
using System.Collections.Generic;
using UI;

public class BossDamagedHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> bossDict = new ();
    private readonly GameObject DamageTextPrefab;

    public string Type => "boss_damaged";

    public BossDamagedHandler(Dictionary<string, GameObject> bossDict,  GameObject DamageTextPrefab)
    {
        this.bossDict = bossDict;
        this.DamageTextPrefab = DamageTextPrefab;
    }

    public void Handle(NetMsg msg)
    {
        BossDamageInfo damagedBoss = msg.damagedBoss;
        if (!bossDict.ContainsKey("boss")) return;
        GameObject boss = bossDict["boss"];
        if (boss == null)
        {
            Debug.LogError("Boss not found");
            return;
        }
        // HP바 업데이트
        GameManager.Instance.UpdateBossHPBar(damagedBoss.currentHp,damagedBoss.maxHp);
        
        //내 플레이어일때만 아웃라인, 데미지 적용
        if (NetworkManager.Instance.MyUserId != damagedBoss.playerId) return;
        //몬스터 아웃라인 셰이더 적용
        var bossController = boss.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.ShowOutline(3f);
        }
        
        // 메인 캔버스 찾기
        Canvas mainCanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        if (mainCanvas != null)
        {
            Debug.Log($"Found main canvas: {mainCanvas.name}");
            
            // 몬스터 위치 가져오기
            Transform damageTextRoot = boss.transform.Find("UICanvas/DamagePos");
            Vector3 worldPos = damageTextRoot != null ? damageTextRoot.position : boss.transform.position;
            
            // 월드 좌표를 스크린 좌표로 변환
            Camera worldCamera = Camera.main;
            Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);
            
            // 스크린 좌표를 UI 좌표로 변환
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform,
                screenPos,
                null, // Overlay 모드
                out canvasPos
            );
            //random pos
            Vector3 newPos = canvasPos;
            int randomAddPos = Random.Range(-20, 20);
            newPos.x += randomAddPos;
            newPos.y += randomAddPos;
            // 메인 캔버스에 직접 생성
            var dmgTextObj = GameObject.Instantiate(DamageTextPrefab, mainCanvas.transform);
            
            // 몬스터 위치에 맞게 설정
            var rectTransform = dmgTextObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localPosition = new Vector3(newPos.x, newPos.y, 0f);
                rectTransform.sizeDelta = new Vector2(200, 100);
            }
            // DamageText 스크립트 초기화
            var dmgText = dmgTextObj.GetComponent<DamageText>();
            if (dmgText != null)
            {
                dmgText.Init(damagedBoss.damage, damagedBoss.isCritical);
            }
        }
    }
}