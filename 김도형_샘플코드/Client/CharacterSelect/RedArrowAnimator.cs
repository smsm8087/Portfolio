using UnityEngine;


public class RedArrowAnimator : MonoBehaviour
{
    public float floatAmplitude = 10f;   // 위아래 이동 폭
    public float floatSpeed = 2f;        // 위아래 속도
    public float rotateSpeed = 90f;      // 초당 회전 속도 (도 단위)

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // 위아래 부드러운 이동 (sin)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

        // Y축 회전
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);
    }
}