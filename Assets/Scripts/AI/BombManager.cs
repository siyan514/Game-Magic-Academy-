// using System.Collections.Generic;
// using UnityEngine;

// public static class BombManager
// {
//     private static List<BombController> activeBombs = new List<BombController>();

//     public static void RegisterBomb(BombController bomb)
//     {
//         if (!activeBombs.Contains(bomb))
//             activeBombs.Add(bomb);
//     }

//     public static void UnregisterBomb(BombController bomb)
//     {
//         activeBombs.Remove(bomb);
//     }

//     public static List<Vector2> GetActiveBombPositions()
//     {
//         List<Vector2> positions = new List<Vector2>();
//         foreach (var bomb in activeBombs)
//         {
//             positions.Add(bomb.transform.position);
//         }
//         return positions;
//     }

//     public bool IsPositionSafe(Vector2 position)
//     {
//         List<Vector2> bombPositions = BombManager.GetActiveBombPositions();
//         foreach (var bombPos in bombPositions)
//         {
//             // 检查水平和垂直方向是否在爆炸范围内
//             bool sameRow = Mathf.Abs(position.y - bombPos.y) < 0.1f;
//             bool sameCol = Mathf.Abs(position.x - bombPos.x) < 0.1f;
//             float distance = sameRow ? Mathf.Abs(position.x - bombPos.x) :
//                              sameCol ? Mathf.Abs(position.y - bombPos.y) : float.MaxValue;

//             // 假设爆炸范围是3（根据实际炸弹范围调整）
//             if (distance <= 3f)
//             {
//                 return false;
//             }
//         }
//         return true;
//     }
// }