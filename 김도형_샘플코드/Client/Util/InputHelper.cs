using UnityEngine;

public static class MovementHelper
{
    public static void Move(Rigidbody2D rb, float direction, float speed)
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction * speed;
        rb.linearVelocity = velocity;
    }

    public static void Jump(Rigidbody2D rb, float jumpForce)
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;
    }
}

public static class InputManager
{
    public static FixedJoystick joystick;

    public static float GetMoveInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.GetAxisRaw("Horizontal"); // 키보드 입력
#elif UNITY_ANDROID || UNITY_IOS
    if (joystick != null && Mathf.Abs(joystick.Horizontal) > 0.01f)
    {
        return joystick.Horizontal; // 조이스틱 입력
    }
    return 0f; // 입력 없음
#else
    return 0f;
#endif
    }
}