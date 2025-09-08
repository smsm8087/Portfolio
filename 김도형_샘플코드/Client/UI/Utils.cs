using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static IEnumerator FadeOut(Image image, float fadeDuration, Action callback = null)
    {
        Color color = image.color;
        float startAlpha = color.a;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalized = t / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, 0f, normalized);
            image.color = color;
            yield return null;
        }

        color.a = 0f;
        image.color = color;
        callback?.Invoke();
    }
    public static IEnumerator FadeIn(Image image, float fadeDuration, Action callback = null)
    {
        Color color = image.color;
        float startAlpha = color.a;
        if(color.a > 0 ) color.a = 0f;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalized = t / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, 1f, normalized);
            image.color = color;
            yield return null;
        }

        color.a = 1f;
        image.color = color;
        callback?.Invoke();
    }
    public static IEnumerator FadeOut(SpriteRenderer image, float fadeDuration, Action callback = null)
    {
        Color color = image.color;
        float startAlpha = color.a;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalized = t / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, 0f, normalized);
            image.color = color;
            yield return null;
        }

        color.a = 0f;
        image.color = color;
        callback?.Invoke();
    }
    public static IEnumerator FadeIn(SpriteRenderer image, float fadeDuration, Action callback = null)
    {
        Color color = image.color;
        float startAlpha = color.a;
        if(color.a > 0 ) color.a = 0f;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalized = t / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, 1f, normalized);
            image.color = color;
            yield return null;
        }

        color.a = 1f;
        image.color = color;
        callback?.Invoke();
    }
    public static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }
}