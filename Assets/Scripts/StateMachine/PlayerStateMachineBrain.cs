/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/02/24
*    Description: After many, many, iterations, this is the state 
*    machine i decided upon. The process is as follows:
*    1. GameManager or Obstacle tiles determine player movement is 
*       needed, if player played a card or stepped on a move tile
*    2. Using the HandleIncomingActions method, this script collects 
*       a list of actions in StartCardActions
*    3. Enters FSM at Prepare next action, which gets the first card
*       in the list
*    4. Goes to FindTileUponAction, which sets targetTile and distance
*    5. Plays a coroutine from PlayerController in PlayResult state
*    6. Gets another card in PrepareNextAction state and repeats, or
*       goes to waiting for actions state.
*       
*       IMPORTANT: AS OF 9/9/24, THERE IS NO HANDLER FOR HITTING 
*       OBSTACLES OR FALLING INTO HOLES/OFF MAP. WIP. 
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachineBrain : MonoBehaviour
{

    [SerializeField] private State currentState;
    private Coroutine currentCoroutine;
    private Card currentAction;
    private List<Card> actions = new List<Card>();
    private Tile targetTile;
    private GameObject player;
    private PlayerController pC;
    private int distance;

    public void Start()
    {
        PlayerController.ReachedDestination += HandleReachedDestination;
        GameManager.PlayActionOrder += HandleIncomingActions;
        TileManager.Instance.LoadTileList();
        FindPlayer();
        //TODO: Have something else load the tileList()
    }

    /// <summary>
    /// Good old enum finite state machine. 
    /// </summary>
    public enum State
    {
        WaitingForActions,
        FindTileUponAction,
        PlayResult,
        PrepareNextAction,
    }

    /// <summary>
    /// This is private because you should not directly influence the FSM
    /// Additionally, you cant have enums as params in animation events,
    /// and that would be the only other reason to be public
    /// </summary>
    /// <param name="stateTo"></param>
    private void FSM(State stateTo)
    {
        switch (stateTo)
        {
            case State.WaitingForActions:
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                print("Waiting for actions");
                currentState = State.WaitingForActions;
                currentCoroutine = StartCoroutine(WaitingForActions());
                break;

            case State.FindTileUponAction:
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentState = State.FindTileUponAction;
                print("Finding target tile");
                currentCoroutine = StartCoroutine(FindTileUponAction());
                break;

            case State.PlayResult:
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentState = State.PlayResult;
                print("Playing results");
                currentCoroutine = StartCoroutine(PlayResult());
                break;

            case State.PrepareNextAction:
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentState = State.PrepareNextAction;
                print("Preparing next action");
                currentCoroutine = StartCoroutine(PrepareNextAction());
                break;
        }
    }

    public void FindPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            pC = GetPlayerController();
            if (pC == null)
            {
                print("NULL");
            }
            if (player == null)
            {
                Debug.Log("No gameobject in scene tagged with player");
            }
        }
    }
    public Card GetNextAction()
    {
        if (actions.Count >= 1)
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
        return GetPlayer().GetComponent<PlayerController>();
    }
    public GameObject GetPlayer()
    {
        if (player != null)
        {
            return player;
        }
        {
            FindPlayer();
            return player;
        }
    }

    public void HandleIncomingActions(List<Card> cardList)
    {
        StartCardActions(cardList);
    }
    public void HandleObstacleInterruption()
    {
        pC.StopFallCoroutine(); //We dont need to stop all these coroutines, but Unity doesnt care
        pC.StopJumpCoroutine(); // and I couldnt figure out how to stop a specific coroutine from
        pC.StopMoveCoroutine(); // another script without making methods

        //TODO : play an interruption animation
        Debug.Log("HIT AN OBSTACLE");

        player.transform.position = pC.GetCurrentTile().GetPlayerSnapPosition();

        FSM(State.PrepareNextAction);
    }
    public void HandleReachedDestination()
    {
        pC.StopFallCoroutine(); //We dont need to stop all these coroutines, but Unity doesnt care
        pC.StopJumpCoroutine(); // and I couldnt figure out how to stop a specific coroutine from
        pC.StopMoveCoroutine(); // another script without making methods. Just to make sure the player is 
        // done moving so there is no jittery behavior

        FSM(State.PrepareNextAction);
    }
    public void SetCardList(List<Card> incomingActions)
    {
        actions.Clear();
        actions.AddRange(incomingActions);
    }


    /// <summary>
    /// The method used to start a new action order from outside scripts
    /// </summary>
    /// <param name="incomingActions"></param>
    public void StartCardActions(List<Card> incomingActions)
    {
        SetCardList(incomingActions);
        FSM(State.PrepareNextAction);
    }

    private IEnumerator PrepareNextAction()
    {
        while (currentState == State.PrepareNextAction)
        {
            currentAction = GetNextAction();
            if (currentAction != null)
            {
                FSM(State.FindTileUponAction);
            }
            else
            {
                FSM(State.WaitingForActions);
            }
            yield return null;
        }
    }

    private IEnumerator FindTileUponAction()
    {
        while (currentState == State.FindTileUponAction)
        {
            var currentTile = pC.GetCurrentTile();

            distance = currentAction.GetDistance(); //main focus of this state
            targetTile = TileManager.Instance.GetTileAtLocation(currentTile, pC.GetCurrentFacingDirection(), distance);

            FSM(State.PlayResult);
            yield return null;
        }
    }

    private IEnumerator PlayResult()
    {
        if (currentState == State.PlayResult)
        {
            switch (currentAction.name)
            {
                case Card.CardName.TurnLeft:
                    pC.TurnPlayer(true);
                    PlayerController.ReachedDestination?.Invoke();
                    //TODO: listen for wait for turn player animation event 
                    break;
                case Card.CardName.TurnRight:
                    pC.TurnPlayer(false);
                    PlayerController.ReachedDestination?.Invoke();
                    //TODO: animation here
                    break;
                case Card.CardName.Jump:
                    if (distance > 2) //this is a spring tile
                    {
                        distance -= 1;
                        //uhhhhhh im counting on spring distance being three, because \/`8 = 2.8... almost 3 tiles. Code wise, i need it to be two
                        // (two up, two across) to work properly
                        int[] possibleNumbers = { 0, 2, 6, 8 };
                        int randomIndex = Random.Range(0, possibleNumbers.Length);

                        targetTile = TileManager.Instance.GetTileAtLocation //TODO: must change from random to targetdirection or direction of targettile
                    (pC.GetCurrentTile(), possibleNumbers[randomIndex], distance);

                        pC.StartJumpCoroutine(pC.GetCurrentTile().GetPlayerSnapPosition(), targetTile.GetPlayerSnapPosition());
                    }

                    else // this is a normal jump
                    {
                        //determine result by getting difference of elevation
                        distance += (pC.GetCurrentTile().GetElevation() - targetTile.GetElevation());

                        if (distance < 0) //block is too high
                        {
                            Vector3 newVector = (targetTile.GetPlayerSnapPosition());
                            pC.StartJumpCoroutine(pC.GetCurrentTile().GetPlayerSnapPosition(), new Vector3(newVector.x, newVector.y - 1, newVector.z));
                        }
                        else
                        {
                            pC.StartJumpCoroutine(pC.GetCurrentTile().GetPlayerSnapPosition(), targetTile.GetPlayerSnapPosition());
                        }
                    }
                    break;
                case Card.CardName.Move:
                    pC.StartMoveCoroutine(pC.GetCurrentTile().GetPlayerSnapPosition(), targetTile.GetPlayerSnapPosition());
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitingForActions()
    {
        while (currentState == State.WaitingForActions)
        {
            yield return null;
        }
    }
}
