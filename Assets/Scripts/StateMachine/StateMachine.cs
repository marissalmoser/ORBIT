/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/1/24
*    Description: 
*******************************************************************/
using UnityEngine;
public class StateMachine
{
    private State currentState;
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
        Debug.LogWarning("Statemachine should be in a state right now");
    }
}