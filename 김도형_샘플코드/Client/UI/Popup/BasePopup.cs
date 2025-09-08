using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    public bool ui_lock { get; set; }
    public virtual void Open()
    {
        gameObject.SetActive(true);
        if (canvasGroup != null)
            StartCoroutine(FadeIn());
    }

    public virtual void Close()
    {
        if (canvasGroup != null)
            StartCoroutine(FadeOutAndDestroy());
        else
            Destroy(gameObject);
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        canvasGroup.alpha = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}