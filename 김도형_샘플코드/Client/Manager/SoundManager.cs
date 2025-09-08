using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Mixer (옵션)")]
    [SerializeField] private AudioMixer mixer;          // 없으면 비워둬도 됨
    [SerializeField] private string musicVolParam = "MusicVol";
    [SerializeField] private string sfxVolParam = "SFXVol";

    [Header("AudioSources")]
    [SerializeField] private AudioSource bgmA;  // loop ON
    [SerializeField] private AudioSource bgmB;  // loop ON
    [SerializeField] private AudioSource sfx;   // loop OFF

    private Dictionary<string, AudioClip> _bgm = new();
    private Dictionary<string, AudioClip> _sfx = new();
    private AudioSource _cur, _next;
    private Coroutine _fadeCo;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _cur = bgmA; _next = bgmB;
        if (_cur) _cur.loop = true;
        if (_next) _next.loop = true;

        foreach (var c in Resources.LoadAll<AudioClip>("Audio/BGM"))
            if (!_bgm.ContainsKey(c.name)) _bgm[c.name] = c;
        foreach (var c in Resources.LoadAll<AudioClip>("Audio/SFX"))
            if (!_sfx.ContainsKey(c.name)) _sfx[c.name] = c;
    }

    // --- BGM ---
    public void PlayBGM(string key, float fade = 0.7f)
    {
        if (!_bgm.TryGetValue(key, out var clip) || clip == null)
        { Debug.LogWarning($"[SoundManagerMini] BGM '{key}' 없음"); return; }
        PlayBGM(clip, fade);
    }

    public void PlayBGM(AudioClip clip, float fade = 0.7f)
    {
        if (!clip) return;
        if (_cur && _cur.clip == clip && _cur.isPlaying) return; // 이미 재생 중
        if (_fadeCo != null) StopCoroutine(_fadeCo);
        _fadeCo = StartCoroutine(Crossfade(clip, fade));
    }

    IEnumerator Crossfade(AudioClip next, float sec)
    {
        var from = _cur; var to = _next;

        to.clip = next; to.volume = 0f; to.loop = true; to.Play();

        float t = 0f, fromStart = from ? from.volume : 0f;
        while (t < sec)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / sec);
            if (from) from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(0f, 1f, k);
            yield return null;
        }
        if (from) { from.Stop(); from.volume = 1f; }

        // swap
        var tmp = _cur; _cur = _next; _next = tmp;
        _fadeCo = null;
    }

    public void StopBGM(float fade = 0.4f)
    {
        if (_cur == null || !_cur.isPlaying) return;
        StartCoroutine(FadeOut(_cur, fade));
    }

    IEnumerator FadeOut(AudioSource src, float sec)
    {
        float start = src.volume, t = 0f;
        while (t < sec)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, 0f, t / sec);
            yield return null;
        }
        src.Stop(); src.volume = 1f;
    }

    // --- SFX ---
    public void PlaySFX(string key, float vol = 1f)
    {
        if (!_sfx.TryGetValue(key, out var clip) || clip == null) { Debug.LogWarning($"SFX '{key}' 없음"); return; }
        sfx.PlayOneShot(clip, vol);
    }
    public void PlaySFX(AudioClip clip, float vol = 1f) { if (clip) sfx.PlayOneShot(clip, vol); }

    // --- Volume ---
    public void SetMusicVolume(float linear01)
    { if (mixer) mixer.SetFloat(musicVolParam, Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f); }
    public void SetSfxVolume(float linear01)
    { if (mixer) mixer.SetFloat(sfxVolParam, Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f); }
}
