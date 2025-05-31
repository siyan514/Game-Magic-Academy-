using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private AIPlayer player;
    // private PathFinding pathFinding;
    // private AIFSM stateMachine;
    // private BombStrategy bombStrategy;

    public void Init(AIPlayer player)
    {
        this.player = player;
        // pathFinding = GetComponent<PathFinding>();
        // stateMachine = new AIFSM(player);
        // bombStrategy = new DefensiveBombStrategy();
    }

    // public Vector2 GetMovement()
    // {
    //     // return stateMachine.CurrentState.CalculateMovement(pathFinding);
    // }

    // public bool ShouldPlaceBomb()
    // {
    //     // return stateMachine.CurrentState.CanPlaceBomb() &&
    //     //        bombStrategy.ShouldPlaceBomb(player, pathFinding);
    // }

    // void Update()
    // {
    //     // stateMachine.UpdateState(); // 驱动状态更新
    // }
}