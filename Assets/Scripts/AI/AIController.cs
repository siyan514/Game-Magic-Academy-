// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class AIController : MonoBehaviour
// {
//     public enum StateType
//     {
//         Chase,
//         Escape,
//         Wander,
//         Collect
//     }

//     private AIPlayer aiPlayer;
//     // private PathFinding pathFinding;
//     private BombStrategy bombStrategy;
//     private List<Vector2> currentPath = new List<Vector2>();
//     private int currentPathIndex = 0;

//     private StateType currentState;

//     public void Init(AIPlayer player)
//     {
//         aiPlayer = player;
//     }

//     private void Start()
//     {
//         ChangeState(StateType.Wander);
//     }

//     private void Update()
//     {
//         if (aiPlayer == null || !aiPlayer.IsActive) return;

//         switch (currentState)
//         {
//             case StateType.Chase:
//                 UpdateChase();
//                 break;
//             case StateType.Escape:
//                 UpdateEscape();
//                 break;
//             case StateType.Wander:
//                 UpdateWander();
//                 break;
//         }
//     }

//     public void ChangeState(StateType newState)
//     {
//         currentState = newState;
//         EnterState(newState);
//     }

//     private void EnterState(StateType state)
//     {
//         switch (state)
//         {
//             case StateType.Chase:
//                 EnterChase();
//                 break;
//             case StateType.Escape:
//                 EnterEscape();
//                 break;
//             case StateType.Wander:
//                 EnterWander();
//                 break;
//         }
//     }


//     #region Path Following
//     public void FollowPath() { }
//     public void SetPath(List<Vector2> path) { }
//     public void ClearPath() { }
//     #endregion

//     #region Wander State
//     private void EnterWander() { }
//     private void UpdateWander() { }
//     private void CheckForWalls() { }
//     #endregion

//     #region Escape State
//     private void EnterEscape() { }
//     private void UpdateEscape() { }
//     #endregion

//     #region Collect State
//     private void EnterCollect() { }
//     private void UpdateCollect() { }
//     #endregion

//     #region Chase State
//     private void EnterChase() { }
//     private void UpdateChase() { }

//     #endregion
// }