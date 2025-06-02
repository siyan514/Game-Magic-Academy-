using UnityEngine;

public class BombStrategy
{
    public bool ShouldPlaceBomb(Vector2 playerPos, Vector2 myPos)
    {
        // 简单策略：当玩家在2个单位距离内且当前位置安全时放置炸弹
        float distance = Vector2.Distance(playerPos, myPos);
        return distance < 2f;
    }
}