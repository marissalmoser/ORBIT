using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStateMachine : MonoBehaviour
{
    [SerializeField] private State currentState;

    private List<Card> actions = new List<Card>();
    private Tile targetTile;
    private GameObject player;
    private PlayerController pC;

    public Card cardReference;

    public void Start()
    {
        PlayerController.OnObstacleInterrupt += HandleObstacleInterruption;
        TileManager.Instance.LoadTileList();
        PlayAction(cardReference);
    }
    public enum State
    {
        WaitingForActions,
        InitializeStateMachine,
        FindTileUponAction,
        DetermineActionResult,
        PlayResult,
        LeaveStateMachine,
        GetNextAction,
    }

    public void FSM(State stateTo)
    {
        switch (stateTo)
        {
            case State.WaitingForActions:
                StartCoroutine(Wait());
                break;
            case State.InitializeStateMachine:
                StartCoroutine(Initialize());
                break;
            case State.FindTileUponAction:
                break;
            case State.DetermineActionResult:
                break;
            case State.PlayResult:

                break;
            case State.GetNextAction:
                if (GetNextAction() != null)
                {

                }
                else
                {

                }
                break;
        }
    }

    public void PlayAction(Card action)
    {
        switch (action.name)
        {
            case Card.CardName.TurnLeft:
                pC.TurnPlayer(true);
                //TODO: animation here
                break;
            case Card.CardName.TurnRight:
                pC.TurnPlayer(false);
                //TODO: animation here
                break;
            case Card.CardName.Jump:
                var currentTile = pC.GetCurrentTile();
                var targetTile = TileManager.Instance.GetTileAtLocation(pC.GetCurrentTile(), pC.GetCurrentFacingDirection(), action.GetDistance());
                pC.StartJumpCoroutine(currentTile, targetTile);
                break;
            case Card.CardName.Move:
                break;
        }
    }
    public Card GetNextAction()
    {
        if (actions.Count > 1)
        {
            Card actionToGive = actions[0];
            actions.RemoveAt(0);
            return actionToGive;
        }
        return null;
    }
    public State GetState()
    {
        return currentState;
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
        if (player != null)
        {
            return player;
        }
        Debug.LogError("Missing a reference to the player");
        return null;
    }

    public void HandleObstacleInterruption()
    {
        pC.StopFallCoroutine(); //We dont need to stop all these coroutines, but Unity doesnt care
        pC.StopJumpCoroutine(); // and I couldnt figure out how to stop a specific coroutine from
        pC.StopMoveCoroutine(); // another script
    }
    public void SetCardList(List<Card> incomingActions)
    {
        actions.Clear();
        actions = incomingActions;
    }
    private IEnumerator Initialize()
    {
        while (currentState == State.InitializeStateMachine)
        {
            yield return null;
        }
    }
    private IEnumerator Wait()
    {
        while (currentState == State.WaitingForActions)
        {
            yield return null;
        }
    }
}
