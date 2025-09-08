using UnityEngine;

public static class AttackBoxHelper
{
    public static NetMsg BuildAttackMessage(BasePlayer player)
    {
        // 공격 범위 박스 위치 계산
        Transform atkTransform = player.attackRangeTransform;
        BoxCollider2D atkCollider = player.attackRangeCollider;

        // 좌우 방향 처리
        Vector3 localPos = atkTransform.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * (player._sr.flipX ? 1f : -1f);
        atkTransform.localPosition = localPos;

        // 월드 좌표 및 스케일 반영
        Vector2 centerWorldPos = atkTransform.position;
        Vector3 lossyScale = atkTransform.lossyScale;
        Vector2 size = atkCollider.size;

        return new NetMsg
        {
            type = "player_attack",
            playerId = NetworkManager.Instance.MyUserId,
            attackBoxCenterX = centerWorldPos.x,
            attackBoxCenterY = centerWorldPos.y,
            attackBoxWidth = size.x * lossyScale.x,
            attackBoxHeight = size.y * lossyScale.y
        };
    }
}