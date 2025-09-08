using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [Header("HP Bar")]
    Image hpImg;
    
   
    
    [Header("Visibility Settings")]
    public float visibilityDuration = 3f; // 체력바가 보이는 시간
    private Coroutine hideCoroutine;
    private bool isVisible = false;
    
    void Awake()
    {
        hpImg = GetComponent<Image>();
        // 시작할 때 체력바 숨기기
        SetVisibility(false);
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (!hpImg) return;
        
        // 데미지를 받았을 때 체력바 표시
        ShowHPBar();
        
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);
        StartCoroutine(LerpHp(healthPercent));
    }

    private void ShowHPBar()
    {
        // 체력바가 보이지 않는 상태라면 표시
        if (!isVisible)
        {
            SetVisibility(true);
        }
        
        // 기존의 숨기기 코루틴이 있다면 중지
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        
        // 일정 시간 후 숨기기
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(visibilityDuration);
        SetVisibility(false);
    }
    
    private void SetVisibility(bool visible)
    {
        isVisible = visible;
        // 부모 HPUI를 숨기기
        transform.parent.gameObject.SetActive(visible);
    }

    public IEnumerator LerpHp(float targetPercent)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float startFill = hpImg.fillAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hpImg.fillAmount = Mathf.Lerp(startFill, targetPercent, elapsed / duration);
            yield return null;
        }

        hpImg.fillAmount = targetPercent;
    }
}