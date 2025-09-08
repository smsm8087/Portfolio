using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("로딩 화면 프리팹")]
    public GameObject loadingScreenPrefab;
    
    private GameObject loadingScreenInstance;
    private Image progressBarImage;
    private UICharacterAnimator characterAnimator;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingScreenPrefab != null)
        {
            loadingScreenInstance = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(loadingScreenInstance);
            loadingScreenInstance.SetActive(false);
            progressBarImage =  loadingScreenInstance.transform.Find("LoadingScreen/progressBar").GetComponent<Image>();
            characterAnimator =  loadingScreenInstance.transform.Find("LoadingScreen").GetComponent<UICharacterAnimator>();
            if (characterAnimator != null)
            {
                characterAnimator.SetJob("loading", new Vector2(0.5f,0.5f));
            }
        }
    }

    /// <summary>
    /// 씬을 비동기로 로드하면서 로딩 화면 표시 및 완료 후 콜백 실행
    /// </summary>
    /// <param name="sceneName">로드할 씬 이름</param>
    /// <param name="onSceneLoaded">씬 로딩 완료 후 실행할 콜백</param>
    public void LoadScene(string sceneName, Action onSceneLoaded = null)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, onSceneLoaded));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Action onSceneLoaded)
    {
        if (loadingScreenInstance != null)
        {
            loadingScreenInstance.SetActive(true);
            // 로딩 시작 알림
            if (PopupManager.Instance != null)
                PopupManager.Instance.SetLoading(true);
        }

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            // 진행도 업데이트 가능: async.progress
            progressBarImage.fillAmount = async.progress;
            yield return null;
        }

        // 로딩 완료되었지만 아직 활성화 전
        yield return new WaitForSeconds(0.5f); // 연출 여유
        progressBarImage.fillAmount = 1f;
        async.allowSceneActivation = true;

        // 씬 완전히 로딩되면 콜백 호출
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (loadingScreenInstance != null)
            {
                loadingScreenInstance.SetActive(false);
                // 로딩 종료 알림
                if (PopupManager.Instance != null)
                    PopupManager.Instance.SetLoading(false);
            }

            onSceneLoaded?.Invoke();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}