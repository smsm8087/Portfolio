using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Image hpImg;
    
    [SerializeField] private Image targetImg; 
    
    void Awake()
    {
        hpImg = GetComponent<Image>();
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (!hpImg) return;
        
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);
        StartCoroutine(LerpHp(healthPercent));
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

        hpImg.fillAmount = targetPercent; // 마지막 값 정확히 맞춤
    }
}