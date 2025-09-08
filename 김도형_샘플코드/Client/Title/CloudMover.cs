using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [Header("이동 속도")]
    public float speed = 0.5f;

    [Header("로컬 X 경계값 (이동 방향 끝)")]
    public float boundaryLocalX = -947.91f;

    [Header("로컬 X 리셋 위치")]
    public float resetLocalX = -972.11f;

    private Vector3 startLocalPos;

    void Start()
    {
        // 시작 위치 저장
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        // 좌표계로 X축 이동
        Vector3 lp = transform.localPosition;
        lp.x += speed * Time.deltaTime;
        transform.localPosition = lp;

        // 경계 체크
        if ((speed > 0 && lp.x > boundaryLocalX) ||
            (speed < 0 && lp.x < boundaryLocalX))
        {
            lp.x = resetLocalX;
            lp.y = startLocalPos.y;
            lp.z = startLocalPos.z;
            transform.localPosition = lp;
        }
    }
}