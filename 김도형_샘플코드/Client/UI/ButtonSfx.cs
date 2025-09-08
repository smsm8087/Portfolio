using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSfx : MonoBehaviour
{
    [Header("��� ���")]
    [SerializeField] private bool useKey = true;     // Ű�� ã����, Ŭ�� ���� ������
    [SerializeField] private string sfxKey = "ui_click";
    [SerializeField] private AudioClip clip;         // useKey=false �� �� ���

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