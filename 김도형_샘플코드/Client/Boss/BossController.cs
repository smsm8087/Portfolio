using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public string guid;
    public Vector3 serverPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject outlineObj;
    [SerializeField] private Transform footPos;
    [SerializeField] private Transform facePos;
    [SerializeField] private Transform bodyPos;
    [SerializeField] private Transform textShowPos;
    [SerializeField] private GameObject summonEffect;

    private bool isSync = false;
    void Start()
    {
        animator.Play("BOSS_idle");
    }

    void Update()
    {
        if (!isSync) return;
        Vector3 dir = serverPosition - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime * 5f);

            if (Mathf.Abs(dir.x) > 0.01f)
            {
                spriteRenderer.flipX = dir.x < 0;
            }
        }
    }
    public void PlayDustSummon()
    {
        SoundManager.Instance.PlaySFX("dustqueen_summon", 0.7f);
        animator.Play("BOSS_summon");
        StartCoroutine(WaitForAnimationThenIdle("BOSS_summon", "BOSS_idle"));
    }

    public void PlayDustSummonEffect(float spawnPosX, float spawnPosY)
    {
        StartCoroutine(FadeOutSummonEffect(spawnPosX, spawnPosY, 1.5f));
    }

    private IEnumerator FadeOutSummonEffect(float spawnPosX, float spawnPosY, float duration)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject eff_obj = Instantiate(summonEffect, new Vector3(spawnPosX, spawnPosY, -1f), Quaternion.identity);
        SpriteRenderer base_renderer = eff_obj.transform.Find("base").GetComponent<SpriteRenderer>();
        SpriteRenderer back_renderer = eff_obj.transform.Find("back").GetComponent<SpriteRenderer>();
        SpriteRenderer front_renderer = eff_obj.transform.Find("front").GetComponent<SpriteRenderer>();
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        renderers.Add(base_renderer);
        renderers.Add(back_renderer);
        renderers.Add(front_renderer);
        
        yield return new WaitForSeconds(duration);
        foreach (SpriteRenderer renderer in renderers)
        {
            StartCoroutine(Utils.FadeOut(renderer, 0.2f));
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(eff_obj);
    }
    
    
    private IEnumerator WaitForAnimationThenIdle(string currentAnim, string nextAnim)
    {
        // 현재 상태 이름이 다를 경우 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(currentAnim))
            yield return null;

        // 현재 애니메이션 길이만큼 대기
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        // 애니메이션 끝나면 idle 전환
        animator.Play(nextAnim);
    }
    
    public void SyncFromServer(float posX)
    {
        serverPosition = new Vector3(posX, transform.position.y, transform.position.z);
        isSync = true;
    }
    public void PlayIntroCoroutine(float hp)
    {
        StartCoroutine(PlayBossIntro(hp));
    }
    private IEnumerator PlayBossIntro(float hp)
    {
        GameManager.Instance.PauseGame();
        GameObject introUI = UIManager.Instance.getIntroUICanvas();
        CanvasGroup canvasGroup = introUI.GetComponent<CanvasGroup>();
        TextMeshProUGUI titleText = introUI.transform.Find("BossTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText =  introUI.transform.Find("BossDesc").GetComponent<TextMeshProUGUI>();
        if(canvasGroup == null || titleText == null || descText == null) yield break;
        canvasGroup.alpha = 0f;
        
        UIManager.Instance.setActiveGameUICanvas(false);
        UIManager.Instance.setActiveIntroUICanvas(true);
        
        //카메라 팔로우
        // 1. 발 → 줌인
        yield return StartCoroutine(CameraFollow.Instance.MoveCamera(footPos.position, 2f, 0.8f));

        // 2. 얼굴 → 유지
        yield return StartCoroutine(CameraFollow.Instance.MoveCamera(facePos.position, 2f, 0.8f));

        // 3. 전체 → 줌아웃
        yield return StartCoroutine(CameraFollow.Instance.MoveCamera(bodyPos.position, 4f, 1.2f));

        yield return StartCoroutine(CameraFollow.Instance.MoveCamera(textShowPos.position, 5f, 0.2f));
        yield return StartCoroutine(PlayBossIntroText(canvasGroup, titleText, descText));   

        //resume 하면 카메라도 알아서 update 됨.
        GameManager.Instance.ResumeGame();
        UIManager.Instance.setActiveGameUICanvas(true);
        UIManager.Instance.setActiveIntroUICanvas(false);
        UIManager.Instance.setActiveBossHpUI(true);
        GameManager.Instance.UpdateBossHPBar(hp,hp);
    }
    IEnumerator PlayBossIntroText(CanvasGroup canvasGroup, TextMeshProUGUI titleText,  TextMeshProUGUI descText)
    {
        // 등장 연출 예시 (페이드 인)
        RectTransform titleRT = titleText.GetComponent<RectTransform>();
        RectTransform descRT = descText.GetComponent<RectTransform>();

        Vector2 titleStartPos = titleRT.anchoredPosition;
        Vector2 descStartPos = descRT.anchoredPosition;

        Vector2 titleDestPos = new Vector2(100, 150);
        Vector2 descDestPos = new Vector2(-150, 0);
        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime * 2f;
            canvasGroup.alpha = Mathf.Clamp01(t); // 페이드 인
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        float titleTime = 0f;
        while (titleTime < 1f)
        {
            titleTime += Time.unscaledDeltaTime;
            float easeT = Utils.EaseOutCubic(titleTime);
            titleRT.anchoredPosition = Vector2.Lerp(titleStartPos, titleDestPos, easeT);
            descRT.anchoredPosition = Vector2.Lerp(descStartPos, descDestPos, easeT);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        canvasGroup.alpha = 0f;
    }
    public void ShowOutline(float duration = 3f)
    {
        if (outlineObj == null) return;

        outlineObj.SetActive(true);
        CancelInvoke(nameof(HideOutline));
        Invoke(nameof(HideOutline), duration);
    }

    private void HideOutline()
    {
        outlineObj.SetActive(false);
    }
}