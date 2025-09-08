using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("팝업이 생성될 부모 Transform")]
    [SerializeField] private Transform popupRoot;

    [Header("팝업 프리팹")]
    [SerializeField] internal GameObject noticePrefab;
    [SerializeField] internal GameObject confirmPrefab;

    // 로딩 중에는 팝업 실행을 지연시킬 큐
    private Queue<Action> deferredPopups = new Queue<Action>();
    private bool isLoadingScene;
    public bool IsLoading => isLoadingScene;
    
    public void EnqueueDeferred(Action action)
    {
        if (action != null)
            deferredPopups.Enqueue(action);
    }

    private void Awake()
    {
        // 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 이 오브젝트와 모든 자식(UI 포함)을 파괴되지 않게 유지
        DontDestroyOnLoad(gameObject);

        if (popupRoot == null)
            Debug.LogError("[PopupManager] popupRoot가 할당되지 않았습니다!");

        SceneManager.sceneLoaded += OnSceneLoaded;
        // 초기 씬 상태 체크
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 기존 씬 이름 기반 판단 (필요 없으면 삭제 가능)
        isLoadingScene = scene.name.Equals("LoadingScene", StringComparison.OrdinalIgnoreCase);
        if (!isLoadingScene)
        {
            // 로딩 끝나면 보류된 팝업 실행
            while (deferredPopups.Count > 0)
            {
                var action = deferredPopups.Dequeue();
                action();
            }
        }
    }

    /// <summary>
    /// 외부에서 로딩 상태를 직접 지정할 수 있도록 공개
    /// </summary>
    public void SetLoading(bool loading)
    {
        isLoadingScene = loading;
        if (!isLoadingScene)
        {
            // 로딩 끝나면 보류된 팝업 실행
            while (deferredPopups.Count > 0)
            {
                var action = deferredPopups.Dequeue();
                action();
            }
        }
    }

    public T ShowPopup<T>(GameObject popupPrefab) where T : BasePopup
    {
        if (isLoadingScene)
        {
            deferredPopups.Enqueue(() => CreatePopup<T>(popupPrefab));
            return null;
        }
        return CreatePopup<T>(popupPrefab);
    }

    private T CreatePopup<T>(GameObject prefab) where T : BasePopup
    {
        if (popupRoot == null)
        {
            Debug.LogError("[PopupManager] popupRoot가 할당되지 않아 팝업을 띄울 수 없습니다!");
            return null;
        }
        var obj = Instantiate(prefab, popupRoot);
        var popup = obj.GetComponent<T>();
        popup.Open();
        return popup;
    }
    private IEnumerator ShowNoticePopupCoroutine(string msg)
    {
        yield return null;
        Action createAndInit = () =>
        {
            var popup = ShowPopup<NoticePopup>(noticePrefab);
            if (popup == null)
                return;  // 로딩 중이라면 일단 무시
            popup.Init(msg,onOk: () => { });
        };

        if (IsLoading) EnqueueDeferred(createAndInit);
        else createAndInit();
    }

    public void ShowNoticePopup(string msg)
    {
        MainThreadUtil.Run(ShowNoticePopupCoroutine(msg));
    }

    private IEnumerator ShowConfirmPopupCoroutine(string msg, string requestId)
    {
        yield return null;
        Action createAndInit = () =>
        {
            var popup = ShowPopup<ConfirmPopup>(confirmPrefab);
            if (popup == null)
                return;  // 로딩 중이라면 일단 무시

            popup.Init(
                msg,
                onYes: () =>
                {
                    NetworkManager.Instance.SendMsg(new NetMsg
                    {
                        type = "confirm_response",
                        requestId = requestId,
                        approved = true
                    });
                },
                onNo: () =>
                {
                    NetworkManager.Instance.SendMsg(new NetMsg
                    {
                        type = "confirm_response",
                        requestId = requestId,
                        approved = false
                    });
                }
            );
        };

        if (IsLoading) EnqueueDeferred(createAndInit);
        else createAndInit();
    }
    public void ShowConfirmPopup(string msg, string requestId)
    {
        MainThreadUtil.Run(ShowConfirmPopupCoroutine(msg, requestId));
    }
}
