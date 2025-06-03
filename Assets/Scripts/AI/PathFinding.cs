// using UnityEngine;
// using System.Collections.Generic;
// using System.Linq;

// public static class Pathfinder
// {
//     public static List<Vector2Int> FindOptimalPath(Vector2Int start, Vector2Int end, LayerMask wallLayer)
//     {
//         // A* 算法实现
//         var openSet = new List<Node>();
//         var closedSet = new HashSet<Vector2Int>();
//         var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

//         var startNode = new Node(start, 0, Heuristic(start, end));
//         openSet.Add(startNode);

//         while (openSet.Count > 0)
//         {
//             // 获取F值最小的节点
//             var currentNode = openSet.OrderBy(n => n.F).First();

//             // 到达终点
//             if (currentNode.Position == end)
//             {
//                 return ReconstructPath(cameFrom, currentNode.Position);
//             }

//             openSet.Remove(currentNode);
//             closedSet.Add(currentNode.Position);

//             // 检查邻居
//             foreach (var neighbor in GetNeighbors(currentNode.Position, wallLayer))
//             {
//                 if (closedSet.Contains(neighbor)) continue;

//                 // 计算G值（移动成本）
//                 float tentativeG = currentNode.G + 1;

//                 var neighborNode = openSet.FirstOrDefault(n => n.Position == neighbor);

//                 if (neighborNode == null)
//                 {
//                     neighborNode = new Node(neighbor, tentativeG, Heuristic(neighbor, end));
//                     openSet.Add(neighborNode);
//                 }
//                 else if (tentativeG >= neighborNode.G)
//                 {
//                     continue;
//                 }

//                 // 记录路径
//                 cameFrom[neighbor] = currentNode.Position;
//                 neighborNode.G = tentativeG;
//             }
//         }

//         // 没有找到路径
//         return null;
//     }

//     private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
//     {
//         var path = new List<Vector2Int> { current };
//         while (cameFrom.ContainsKey(current))
//         {
//             current = cameFrom[current];
//             path.Insert(0, current);
//         }
//         return path;
//     }

//     private static float Heuristic(Vector2Int a, Vector2Int b)
//     {
//         // 曼哈顿距离
//         return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
//     }

//     private static IEnumerable<Vector2Int> GetNeighbors(Vector2Int position, LayerMask wallLayer)
//     {
//         Vector2Int[] directions = {
//             Vector2Int.up,
//             Vector2Int.down,
//             Vector2Int.left,
//             Vector2Int.right
//         };

//         foreach (var dir in directions)
//         {
//             Vector2Int neighbor = position + dir;

//             // 检查是否是墙
//             Collider2D wall = Physics2D.OverlapPoint(new Vector2(neighbor.x, neighbor.y), wallLayer);
//             if (wall == null)
//             {
//                 yield return neighbor;
//             }
//         }
//     }

//     private class Node
//     {
//         public Vector2Int Position { get; }
//         public float G { get; set; } // 从起点到当前节点的成本
//         public float H { get; }       // 启发式估计值
//         public float F => G + H;     // 总成本

//         public Node(Vector2Int position, float g, float h)
//         {
//             Position = position;
//             G = g;
//             H = h;
//         }
//     }
// }