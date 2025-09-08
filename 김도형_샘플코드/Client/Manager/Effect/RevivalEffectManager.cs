using UnityEngine;
using System.Collections.Generic;

public class RevivalEffectManager : MonoBehaviour
{
    public static RevivalEffectManager Instance { get; private set; }
    
    [Header("Effect Prefabs")]
    public GameObject revivalEffectPrefab;
    
    [Header("Audio")]
    public AudioClip revivalStartSound;
    public AudioClip revivalCompleteSound;
    
    private Dictionary<string, GameObject> activeEffects = new Dictionary<string, GameObject>();
    private AudioSource audioSource;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    /// <summary>
    /// 부활 이펙트 시작
    /// </summary>
    public void StartRevivalEffect(string targetPlayerId, Vector3 position)
    {
        Debug.Log($"[RevivalEffect] 부활 이펙트 시작: {targetPlayerId}");
        
        // 이미 이펙트가 있으면 제거
        if (activeEffects.ContainsKey(targetPlayerId))
        {
            StopRevivalEffect(targetPlayerId);
        }
        
        // 새 이펙트 생성
        if (revivalEffectPrefab != null)
        {
            GameObject effect = Instantiate(revivalEffectPrefab, position, Quaternion.identity);
            activeEffects[targetPlayerId] = effect;
            
            // 이펙트 시작
            RevivalEffectController controller = effect.GetComponent<RevivalEffectController>();
            if (controller != null)
            {
                controller.StartEffect();
            }
            
            // 사운드 재생
            if (revivalStartSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(revivalStartSound);
            }
        }
    }
    
    /// <summary>
    /// 부활 이펙트 중단
    /// </summary>
    public void StopRevivalEffect(string targetPlayerId)
    {
        Debug.Log($"[RevivalEffect] 부활 이펙트 중단: {targetPlayerId}");
        
        if (activeEffects.TryGetValue(targetPlayerId, out GameObject effect))
        {
            if (effect != null)
            {
                Destroy(effect);
            }
            activeEffects.Remove(targetPlayerId);
        }
    }
    
    /// <summary>
    /// 부활 완료 처리
    /// </summary>
    public void PlayRevivalCompleteEffect(Vector3 position)
    {
        Debug.Log($"[RevivalEffect] 부활 완료 - 사운드 재생");
        
        // 완료 사운드만 재생
        if (revivalCompleteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(revivalCompleteSound);
        }
    }
}