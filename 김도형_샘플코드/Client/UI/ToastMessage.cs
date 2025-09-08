using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class ToastMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI toastText; 
        [SerializeField] private Image backgroundImage;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Animation Settings")]
        [SerializeField] private float holdDuration = 2f; 
        [SerializeField] private float fadeDuration = 2f;
        
        private Coroutine currentToastCoroutine;
        
        private void Awake()
        {
            // 자식에서 컴포넌트 찾기
            if (toastText == null) toastText = GetComponentInChildren<TextMeshProUGUI>();
            if (backgroundImage == null) backgroundImage = GetComponentInChildren<Image>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            
            // 초기에는 토스트 전체 숨김
            gameObject.SetActive(false);
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(false);
        }

        public void ShowToast(string message)
        {
            Debug.Log($"ShowToast called with: {message}");
    
            // 기존 코루틴 중단
            if (currentToastCoroutine != null)
            {
                StopCoroutine(currentToastCoroutine);
            }
    
            gameObject.SetActive(true);
            
            // 배경 활성화
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(true);
                
            toastText.text = message;
            
            // 알파값 초기화
            canvasGroup.alpha = 1f;
            
            currentToastCoroutine = StartCoroutine(ToastSequence());
        }

        private IEnumerator ToastSequence()
        {
            yield return new WaitForSeconds(holdDuration);
            
            // 페이드아웃 시작
            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;
            
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
                canvasGroup.alpha = alpha;
                yield return null;
            }
            
            // 완료 후 정리
            canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
            currentToastCoroutine = null;
        }

        public void HideToast()
        {
            // 코루틴 중단
            if (currentToastCoroutine != null)
            {
                StopCoroutine(currentToastCoroutine);
                currentToastCoroutine = null;
            }
            
            // 토스트 배경도 숨김
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(false);
                
            canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
        }
    }
}