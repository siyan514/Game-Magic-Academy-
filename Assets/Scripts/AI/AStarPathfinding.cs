using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A*寻路算法实现类
/// </summary>
public static class AStarPathfinding
{
    /// <summary>
    /// 寻路节点类
    /// </summary>
    private class PathNode
    {
        public Vector2Int position;     // 节点位置
        public float gCost;            // 从起点到当前节点的实际代价
        public float hCost;            // 从当前节点到终点的启发式代价
        public float fCost => gCost + hCost;  // 总代价
        public PathNode parent;        // 父节点，用于重建路径

        public PathNode(Vector2Int pos)
        {
            position = pos;
            gCost = 0;
            hCost = 0;
            parent = null;
        }
    }

    /// <summary>
    /// 使用A*算法寻找从起点到终点的最优路径
    /// </summary>
    /// <param name="startPos">起始位置</param>
    /// <param name="targetPos">目标位置</param>
    /// <returns>路径点列表，如果找不到路径返回null</returns>
    public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        // 开放列表：待评估的节点
        List<PathNode> openList = new List<PathNode>();
        // 关闭列表：已评估的节点
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // 创建起始节点
        PathNode startNode = new PathNode(startPos);
        startNode.gCost = 0;
        startNode.hCost = CalculateHeuristic(startPos, targetPos);
        openList.Add(startNode);

        // 主循环：持续寻路直到找到路径或开放列表为空
        while (openList.Count > 0)
        {
            // 从开放列表中选择F代价最小的节点
            PathNode currentNode = GetLowestFCostNode(openList);
            
            // 将当前节点从开放列表移至关闭列表
            openList.Remove(currentNode);
            closedList.Add(currentNode.position);

            // 如果到达目标位置，重建并返回路径
            if (currentNode.position == targetPos)
            {
                return ReconstructPath(currentNode);
            }

            // 检查当前节点的所有邻居
            foreach (Vector2Int neighborPos in GetNeighbors(currentNode.position))
            {
                // 跳过已在关闭列表中的邻居
                if (closedList.Contains(neighborPos))
                    continue;

                // 跳过不可通行的邻居（墙壁等）
                if (!IsWalkable(neighborPos))
                    continue;

                // 计算到邻居节点的新G代价
                float tentativeGCost = currentNode.gCost + GetMovementCost(currentNode.position, neighborPos);

                // 查找邻居是否已在开放列表中
                PathNode neighborNode = openList.FirstOrDefault(n => n.position == neighborPos);

                if (neighborNode == null)
                {
                    // 邻居不在开放列表中，创建新节点
                    neighborNode = new PathNode(neighborPos);
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateHeuristic(neighborPos, targetPos);
                    neighborNode.parent = currentNode;
                    openList.Add(neighborNode);
                }
                else if (tentativeGCost < neighborNode.gCost)
                {
                    // 找到到邻居的更短路径，更新邻居节点
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.parent = currentNode;
                }
            }
        }

        // 未找到路径
        return null;
    }

    /// <summary>
    /// 从开放列表中获取F代价最小的节点
    /// </summary>
    private static PathNode GetLowestFCostNode(List<PathNode> openList)
    {
        PathNode lowestNode = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].fCost < lowestNode.fCost)
            {
                lowestNode = openList[i];
            }
        }
        return lowestNode;
    }

    /// <summary>
    /// 计算启发式代价（曼哈顿距离）
    /// </summary>
    private static float CalculateHeuristic(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }

    /// <summary>
    /// 获取指定位置的所有邻居位置（上下左右四个方向）
    /// </summary>
    private static Vector2Int[] GetNeighbors(Vector2Int position)
    {
        return new Vector2Int[]
        {
            position + Vector2Int.up,      // 上
            position + Vector2Int.down,    // 下
            position + Vector2Int.left,    // 左
            position + Vector2Int.right    // 右
        };
    }

    /// <summary>
    /// 检查指定位置是否可通行
    /// </summary>
    private static bool IsWalkable(Vector2Int position)
    {
        Vector2 worldPos = new Vector2(position.x, position.y);
        
        // 检查是否有墙壁碰撞体
        Collider2D wallCollider = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("wall"));
        if (wallCollider != null)
            return false;

        // 检查超级墙
        if (GameController.instance != null && GameController.instance.IsSuperWall(worldPos))
            return false;

        return true;
    }

    /// <summary>
    /// 计算从一个位置移动到邻居位置的代价
    /// </summary>
    private static float GetMovementCost(Vector2Int from, Vector2Int to)
    {
        // 格子游戏中，相邻格子的移动代价为1
        return 1f;
    }

    /// <summary>
    /// 从终点节点重建完整路径
    /// </summary>
    private static List<Vector2Int> ReconstructPath(PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode currentNode = endNode;

        // 从终点向起点回溯，构建路径
        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        // 反转路径，使其从起点到终点
        path.Reverse();
        return path;
    }
}