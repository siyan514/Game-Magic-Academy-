// public abstract class BombStrategy
// {
//     public abstract bool ShouldPlaceBomb(AIPlayer player, PathFinding pathFinding);
// }

// // 保守策略：只在安全时放炸弹
// public class DefensiveBombStrategy : BombStrategy
// {
//     public override bool ShouldPlaceBomb(AIPlayer player, PathFinding pathFinding)
//     {
//         return player.BombCount > 0 &&
//                pathFinding.HasSafeEscape();
//     }
// }

// // 激进策略：主动攻击
// public class AggressiveBombStrategy : BombStrategy
// {
//     public override bool ShouldPlaceBomb(AIPlayer player, PathFinding pathFinding)
//     {
//         return player.BombCount > 0 &&
//                PlayerInRange(player);
//     }
// }