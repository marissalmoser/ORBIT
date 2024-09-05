/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/1/24
*    Description: 
*******************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class StateMachineOld
{
    private State currentState;
    private List<Card> actions;
    public static UnityAction ActionComplete;
    private GameObject player;
    private PlayerController pC;
    public StateMachineOld(List<Card> incomingActions, GameObject playerGO)
    {
        actions.Clear();
        actions = incomingActions;
        player = playerGO;
        pC = player.GetComponent<PlayerController>();
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

    public PlayerController GetPlayerController()
    {
        if (pC != null)
        {
            return pC;
        }
        Debug.LogError("Missing a reference to the player script");
        return null;
    }
    public GameObject GetPlayer()
    {
        if(player != null)
        {
            return player;
        }
        Debug.LogError("Missing a reference to the player");
        return null;
    }
    public void SetState(State state)
    {
        currentState = state;
    }
    public void Run()
    {
        if (currentState != null)
        {
            currentState.Execute(this);
        }
        Debug.LogWarning("Statemachine should be in a state right now");
    }

    public Card GetCurrentAction()
    {
        return actions[0];
    }
    public Card GetNextAction()
    {
        if (actions.Count > 1)
        {
            actions.RemoveAt(0); //remove first action in order
            
            return actions[0];
        }
        else
        {
            EndStateMachine();
            return null;
        }
    }
    public void EndStateMachine()
    {
        //Alert GameManager here
    }
}