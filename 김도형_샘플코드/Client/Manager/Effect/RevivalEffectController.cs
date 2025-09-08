using UnityEngine;
using System.Collections;

public class RevivalEffectController : MonoBehaviour
{
    [Header("Effect Components")]
    public Animator effectAnimator;
    
    [Header("Animation Settings")]
    public float animationSpeed = 1f;
    
    public void StartEffect()
    {
        if (effectAnimator != null)
        {
            // 애니메이션 속도 조절
            effectAnimator.speed = animationSpeed;
            
            // 애니메이션 재생
            effectAnimator.enabled = true;
        }
    }
}