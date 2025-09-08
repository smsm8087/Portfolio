using System.Collections;
using System.Collections.Generic;
using DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CenterText : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI centerText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private ToastMessage toastMessage;
        
        private void Awake()
        {
            // 자식에서 컴포넌트 찾기
            if (centerText == null) centerText = GetComponentInChildren<TextMeshProUGUI>();
            if (backgroundImage == null) backgroundImage = GetComponentInChildren<Image>();
            if (toastMessage == null) toastMessage = GetComponentInChildren<ToastMessage>();
            
            // 초기에는 배경과 텍스트 모두 숨김
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(false);
            if (centerText != null)
                centerText.enabled = false;
        }

        public void UpdateText(int count, string start_msg)
        {
            gameObject.SetActive(true);

            if (count > 0)
            {
                // 카운트다운
                toastMessage.HideToast();
                
                if (backgroundImage != null)
                    backgroundImage.gameObject.SetActive(true);
                
                if (centerText != null)
                {
                    centerText.enabled = true;
                    centerText.text = count.ToString();
                }
            }
            else if (!string.IsNullOrEmpty(start_msg))
            {
                // 메시지가 있는 경우
                if (start_msg == "start!!")
                {
                    if (backgroundImage != null)
                        backgroundImage.gameObject.SetActive(true);
                    
                    if (centerText != null)
                    {
                        centerText.enabled = true;
                        centerText.text = start_msg;
                    }
                }
                else if (start_msg.StartsWith("Wave"))
                {
                    // 웨이브 메시지는 토스트로
                    if (backgroundImage != null)
                        backgroundImage.gameObject.SetActive(false);
                    
                    if (centerText != null)
                        centerText.enabled = false;
                    
                    toastMessage.ShowToast(start_msg);
                }
                else
                {
                    // 기타 메시지는 배경과 함께 고정 텍스트로
                    if (backgroundImage != null)
                        backgroundImage.gameObject.SetActive(true);
                    
                    if (centerText != null)
                    {
                        centerText.enabled = true;
                        centerText.text = start_msg;
                    }
                }
            }
            else
            {
                // 둘 다 없으면 모두 숨김
                if (backgroundImage != null)
                    backgroundImage.gameObject.SetActive(false);
                
                if (centerText != null)
                    centerText.enabled = false;
                
                toastMessage.HideToast();
            }
        }
        
        // 고정 텍스트 숨기기
        public void HideCenterText()
        {
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(false);
            
            if (centerText != null)
                centerText.enabled = false;
        }
    }
}