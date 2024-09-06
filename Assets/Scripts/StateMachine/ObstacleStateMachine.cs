using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStateMachine : MonoBehaviour
{
    [SerializeField] private State currentState;
    private Coroutine currentCoroutine;
    private Card currentAction;

    private List<Card> actions = new List<Card>();
    private Tile targetTile;
    private GameObject player;
    private PlayerController pC;


    public Card cardReference;

    public void Start()
    {
        PlayerController.ReachedDestination += HandleObstacleInterruption;
        TileManager.Instance.LoadTileList();
        FindPlayer();
    }
    public enum State
    {
        WaitingForActions,
        Initialize,
        FindTileUponAction,
        DetermineActionResult,
        PlayResult,
        PrepareNextAction,
        LeaveStateMachine,
    }

    public void FSM(State stateTo)
    {
        switch (stateTo)
        {
            case State.WaitingForActions:
                StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(WaitingForActions());
                break;

            //case State.Initialize:
            //    StopCoroutine(currentCoroutine);
            //    currentCoroutine = StartCoroutine(Initialize());
            //    break;

            case State.DetermineActionResult:
                StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(DetermineActionResult());
                break;

            case State.PlayResult:
                StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(PlayResult());
                break;

            case State.PrepareNextAction:
                StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(PrepareNextAction());
                break;
        }
    }

    public void FindPlayer()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if(player == null)
            {
                Debug.Log("No gameobject in scene tagged with player");
            }
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
    public void HandleObstacleInterruption()
    {
        pC.StopFallCoroutine(); //We dont need to stop all these coroutines, but Unity doesnt care
        pC.StopJumpCoroutine(); // and I couldnt figure out how to stop a specific coroutine from
        pC.StopMoveCoroutine(); // another script without making methods
    }
    public void SetCardList(List<Card> incomingActions)
    {
        actions.Clear();
        actions = incomingActions;
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



    //private IEnumerator Initialize()
    //{
    //    while (currentState == State.Initialize)
    //    {

    //        yield return null;
    //    }
    //}

    private IEnumerator PrepareNextAction()
    {
        while (currentState == State.PrepareNextAction)
        {
            currentAction = GetNextAction();
            FSM(State.FindTileUponAction);
            yield return null;
        }
    }

    private IEnumerator FindTileUponAction()
    {
        while (currentState == State.FindTileUponAction)
        {
            var currentTile = pC.GetCurrentTile();

            int distance = currentAction.GetDistance();
            int additionalDistance = currentTile.GetElevation() - targetTile.GetElevation();
            targetTile = TileManager.Instance.GetTileAtLocation
                (pC.GetCurrentTile(), pC.GetCurrentFacingDirection(), currentAction.GetDistance());

            FSM(State.DetermineActionResult);
            yield return null;
        }
    }

    private IEnumerator DetermineActionResult()
    {
        while (currentState == State.DetermineActionResult)
        {
            switch (currentAction.name)
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
                    //TODO: start animation here
                    var obstacle = targetTile.GetObstacleClass();
                    if(obstacle == null) //no obstacle
                    {

                    }
                    else
                    {
                        
                    }
                    break;
                case Card.CardName.Move:
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator PlayResult()
    {
        while(currentState == State.PlayResult)
        {
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
