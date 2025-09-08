using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameResultPoup : BasePopup
{
    [SerializeField] GameObject TitleObj;
    [SerializeField] GameObject InfoObj;
    [SerializeField] CanvasGroup InfoCanvasGroup;
    [SerializeField] GameObject ClearObj;
    [SerializeField] GameObject GameOverObj;
    [SerializeField] Button outButton;

    public void Init(string resultType)
    {
        InfoCanvasGroup.alpha = 0f;
        transform.localPosition =  new Vector3(0, -100f, 0);
        Vector3 titlePos = TitleObj.transform.localPosition;
        Vector3 infoPos = InfoObj.transform.localPosition;
        switch (resultType)
        {
            case "clear":
            {
                ClearObj.SetActive(true);
                GameOverObj.SetActive(false);
            }
            break;
            case "gameover":
            {
                GameOverObj.SetActive(true);
                ClearObj.SetActive(false);
            }
            break;
        }
        outButton.onClick.AddListener(()=>
        {
            base.Close();
            NetworkManager.Instance.Reset();
            SceneLoader.Instance.LoadScene("LobbyScene");
        });
        TitleObj.transform.localPosition += new Vector3(0f, 270f, 0f);
        StartCoroutine(TitleObj.transform.localPosition, titlePos, TitleObj);
        InfoObj.transform.localPosition += new Vector3(0f, -100f, 0f);
        StartCoroutine(InfoObj.transform.localPosition, infoPos, InfoObj,  InfoCanvasGroup);
    }
    public void StartCoroutine(Vector3 fromPos, Vector3 toPos, GameObject target, CanvasGroup canvasGroup = null)
    {
        StartCoroutine(StartAnimation(fromPos,toPos, target, canvasGroup));
    }
    private IEnumerator StartAnimation(Vector3 fromPos, Vector3 toPos, GameObject target, CanvasGroup canvasGroup = null)
    {
        float duration = 0.5f;
        float time = 0f;
        
        target.transform.localPosition = fromPos;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);

            float eased = Utils.EaseOutCubic(t); 
            target.transform.localPosition = Vector3.Lerp(fromPos, toPos, eased);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = eased;
            }
            yield return null;
        }

        target.transform.localPosition = toPos;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }
}
