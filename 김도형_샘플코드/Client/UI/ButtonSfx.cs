using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSfx : MonoBehaviour
{
    [Header("재생 방식")]
    [SerializeField] private bool useKey = true;     // 키로 찾을지, 클립 직접 넣을지
    [SerializeField] private string sfxKey = "ui_click";
    [SerializeField] private AudioClip clip;         // useKey=false 일 때 사용

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClickPlay);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickPlay);
    }

    private void OnClickPlay()
    {
        if (useKey)
        {
            SoundManager.Instance.PlaySFX(sfxKey);
        }
        else if (clip != null)
        {
            SoundManager.Instance.PlaySFX(clip);
        }
    }
}