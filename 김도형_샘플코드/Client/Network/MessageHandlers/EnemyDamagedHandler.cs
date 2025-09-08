using UnityEngine;
using System.Collections.Generic;
using UI;
using UnityEngine.UI;

public class EnemyDamagedHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> Enemies = new ();
    private readonly GameObject DamageTextPrefab;

    public string Type => "enemy_damaged";

    public EnemyDamagedHandler(Dictionary<string, GameObject> Enemies,  GameObject DamageTextPrefab)
    {
        this.Enemies = Enemies;
        this.DamageTextPrefab = DamageTextPrefab;
    }

    public void Handle(NetMsg msg)
    {
        List<EnemyDamageInfo> damagedEnemies = msg.damagedEnemies;
        foreach (var damagedEnemy in damagedEnemies)
        {
            var pid = damagedEnemy.enemyId;
            if (Enemies.TryGetValue(pid, out var enemyObj))
            {
                //몬스터 아웃라인 셰이더 적용
                var enemyController = enemyObj.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.ShowOutline(3f);
                }
                // HP바 업데이트
                Transform hpBarFill = enemyObj.transform.Find("UICanvas/HpUI/Health Bar Fill");
                if (hpBarFill != null)
                {
                    var enemyHpBar = hpBarFill.GetComponent<EnemyHPBar>();
                    if (enemyHpBar != null)
                    {
                        enemyHpBar.UpdateHP(damagedEnemy.currentHp, damagedEnemy.maxHp);
                    }
                    
                }
                // 메인 캔버스 찾기
                Canvas mainCanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
                if (mainCanvas != null)
                {
                    Debug.Log($"Found main canvas: {mainCanvas.name}");
                    
                    // 몬스터 위치 가져오기
                    Transform damageTextRoot = enemyObj.transform.Find("UICanvas/DamagePos");
                    Vector3 worldPos = damageTextRoot != null ? damageTextRoot.position : enemyObj.transform.position;
                    
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
                        dmgText.Init(damagedEnemy.damage, damagedEnemy.isCritical);
                    }
                }
            }
        }
    }
}