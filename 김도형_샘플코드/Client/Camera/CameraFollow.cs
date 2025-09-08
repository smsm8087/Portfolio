using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    public float lerpSpeed = 1.5f;
    
    [Header("Camera Bounds")]
    public float minX = -20f; 
    public float maxX = 20f;  
    public float minY = 0f;   
    public float maxY = 10f;  
    
    private Transform target;

    private void Awake()
    {
        if(Instance != null) Destroy(this);
        Instance = this;
    }

    public void setTarget(Transform _target)
    {
        target = _target;
    }
    private void Update()
    {
        if (target == null) return;
        
        // 기본 타겟 위치 계산
        
        float targetX = target.position.x;
        float targetY = Mathf.Max(target.position.y, minY);
        
        // 경계 제한 적용
        targetX = Mathf.Clamp(targetX, minX, maxX);
        targetY = Mathf.Clamp(targetY, minY, maxY);
        
        Vector3 targetPos = new Vector3(targetX, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }

    public IEnumerator ResetCamera()
    {
        var cam = Camera.main;
        if (cam.orthographicSize != 5f)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / 0.2f;

                float easeT = Utils.EaseOutCubic(t);
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 5f, easeT);
                yield return null;
            }

            cam.orthographicSize = 5f;
        }
    }
    public IEnumerator MoveCamera(Vector3 targetPos, float targetSize, float duration)
    {
        Camera cam = Camera.main;
        Vector3 startPos = cam.transform.position;
        Vector3 endPos = new Vector3(targetPos.x, targetPos.y, startPos.z);

        float startSize = cam.orthographicSize;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;

            float easeT = Utils.EaseOutCubic(t);
            cam.transform.position = Vector3.Lerp(startPos, endPos, easeT);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, easeT);
            yield return null;
        }

        cam.transform.position = endPos;
        cam.orthographicSize = targetSize;
    }
}