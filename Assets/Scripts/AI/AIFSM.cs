using UnityEngine;

public class AIFSM
{
    public AIState CurrentState { get; private set; }
    private readonly AIPlayer player;
    private readonly AIController controller;

    public AIFSM(AIPlayer player, AIController controller)
    {
        this.player = player;
        this.controller = controller;
        TransitionTo(new WanderState());
    }

    public void TransitionTo(AIState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter(player, controller);
    }

    public void UpdateState()
    {
        CurrentState.Update();
    }
}

public abstract class AIState
{
    public abstract void Enter(AIPlayer player, AIController controller);
    public abstract void Exit();
    public abstract void Update();
}