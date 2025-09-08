using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class NetworkCharacterFollower : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 10f;

    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;
    private bool lastFacingRight = true;
    private bool firstSync = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
    }

    public void SetTargetPosition(Vector3 newPos)
    {
        firstSync = false;
        float dx = newPos.x - transform.position.x;

        if (Mathf.Abs(dx) > 0.01f)
        {
            lastFacingRight = dx > 0;
        }

        spriteRenderer.flipX = lastFacingRight;
        targetPosition = newPos;
    }
    private void Update()
    {
        if (!firstSync)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
        }
    }
}