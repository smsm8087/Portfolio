using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace UI
{
    public class DamageText : MonoBehaviour
    {
        private RectTransform rectTransform;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private GameObject criticalDamageImage;

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Init(int damage, bool isCritical)
        {
            // 텍스트 설정
            damageText.text = damage.ToString();
            damageText.gameObject.SetActive(true);
            if (isCritical)
            {
                criticalDamageImage.SetActive(true);
                damageText.color = Color.red;
            }
            
            Debug.Log($"DamageText initialized: {damage}, color: {damageText.color}");

            // UI 좌표계에서 위로 이동할 목표 위치
            Vector3 targetPos = rectTransform.localPosition + new Vector3(0, 50f, 0);

            StartCoroutine(PlayDamageAnim(targetPos));
        }

        private IEnumerator PlayDamageAnim(Vector3 targetPos)
        {
            Vector3 startPos = rectTransform.localPosition;
            float duration = 0.5f; 
            float elapsed = 0f;
            
            Color originalColor = damageText.color;
            Debug.Log($"Animation started from {startPos} to {targetPos}");

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 위치 이동
                rectTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);

                // 알파 조절 
                Color c = originalColor;
                c.a = Mathf.Lerp(1f, 0f, t * 0.5f);
                damageText.color = c;

                yield return null;
            }

            Debug.Log("DamageText animation completed, destroying object");
            Destroy(gameObject);
        }
    }
}