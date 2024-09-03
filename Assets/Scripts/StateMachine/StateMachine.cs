/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/1/24
*    Description: 
*******************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardAction : ScriptableObject
{
    //placeholder until i get ryans SOs
}
public class StateMachine
{
    private State currentState;
    private List<CardAction> actions;
    private static UnityAction StateMachineComplete;
    public StateMachine(List<CardAction> incomingActions)
    {
        actions.Clear();
        actions = incomingActions;
    }

    public State GetCurrentState()
    {
        if (currentState != null)
        {
            return currentState;
        }
        Debug.LogError("There is no current state");
        return null;
    }
    public void SetState(State state)
    {
        currentState = state;
    }
    public void Run()
    {
        while (currentState != null)
        {
            currentState.Execute(this);
        }
        if (currentState is ExitStateMachine)
        {
            currentState = GetNextAction();
        }
        Debug.LogWarning("Statemachine should be in a state right now");
    }
    public State GetNextAction()
    {
        return new ExitStateMachine();
    }
    public void EndStateMachine()
    {
        StateMachineComplete.Invoke();
    }
}