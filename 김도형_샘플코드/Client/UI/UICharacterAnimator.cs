using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterAnimator : MonoBehaviour
{
    [Header("UI Components")]
    public Image targetImage;

    [Header("Animation Data List")]
    public List<JobIdleAnimationData> jobAnimationDataList;

    private List<FrameData> frames;
    private int currentIndex = 0;
    private float timer = 0f;
    
    [SerializeField] private Vector2 baseAnchoredPosition;

    private void Update()
    {
        if (frames == null || frames.Count == 0 || targetImage == null) return;

        timer += Time.deltaTime;

        if (timer >= frames[currentIndex].duration)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % frames.Count;
            Sprite nextSprite = frames[currentIndex].sprite;

            // Sprite가 같더라도 강제로 갱신시키기 위해 임시 null 처리
            if (targetImage.sprite == nextSprite)
            {
                targetImage.sprite = null;
            }

            targetImage.sprite = nextSprite;
        }
    }
    public void SetJob(string jobType)
    {
        targetImage.sprite = null;
        var data = jobAnimationDataList.FirstOrDefault(j => j.jobType.ToLower() == jobType.ToLower());

        if (data == null)
        {
            Debug.LogWarning($"[SetJob] 해당 직업({jobType})의 애니메이션 데이터가 없습니다.");
            frames = null;
            return;
        }
        targetImage.rectTransform.pivot = new Vector2(0.5f, 0f);

        frames = data.idleFrames;
        currentIndex = 0;
        timer = 0f;

        if (frames.Count > 0 && targetImage != null)
            targetImage.sprite = frames[0].sprite;
        
    }
    public void SetJob(string jobType, Vector2 forcePivotPos)
    {
        targetImage.sprite = null;
        var data = jobAnimationDataList.FirstOrDefault(j => j.jobType.ToLower() == jobType.ToLower());

        if (data == null)
        {
            Debug.LogWarning($"[SetJob] 해당 직업({jobType})의 애니메이션 데이터가 없습니다.");
            frames = null;
            return;
        }
        targetImage.rectTransform.pivot = forcePivotPos;

        frames = data.idleFrames;
        currentIndex = 0;
        timer = 0f;

        if (frames.Count > 0 && targetImage != null)
            targetImage.sprite = frames[0].sprite;

    }
}