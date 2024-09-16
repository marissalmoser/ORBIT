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

    [SerializeField] private State _currentState;
    private Coroutine _currentStateCoroutine;
    private Card _currentAction;
    private List<Card> _actions = new List<Card>();
    private Tile _targetTile;
    private GameObject _player;
    private PlayerController _pC;
    private int _distance;
    private bool _firedTraps = false;
    public void Start()
    {
        PlayerController.ReachedDestination += HandleReachedDestination;
        PlayerController.AddCard += HandleCardAdd;
        PlayerController.SpikeCollision += HandleSpikeInterruption;
        PlayerController.WallInterruptAnimation += HandleWallInterruption;
        GameManager.PlayActionOrder += HandleIncomingActions;
        TileManager.Instance.LoadTileList();
        TileManager.Instance.LoadObstacleList();
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
        TrapPlayState,
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
                if (_currentStateCoroutine != null)
                {
                    StopCoroutine(_currentStateCoroutine);
                }
                print("Waiting for actions");
                GameManager.Instance.NewTurn();
                _currentState = State.WaitingForActions;
                _currentStateCoroutine = StartCoroutine(WaitingForActions());
                break;

            case State.FindTileUponAction:
                if (_currentStateCoroutine != null)
                {
                    StopCoroutine(_currentStateCoroutine);
                }
                _currentState = State.FindTileUponAction;
                print("Finding target tile");
                _currentStateCoroutine = StartCoroutine(FindTileUponAction());
                break;

            case State.PlayResult:
                if (_currentStateCoroutine != null)
                {
                    StopCoroutine(_currentStateCoroutine);
                }
                _currentState = State.PlayResult;
                print("Playing results");
                _currentStateCoroutine = StartCoroutine(PlayResult());
                break;

            case State.PrepareNextAction:
                if (_currentStateCoroutine != null)
                {
                    StopCoroutine(_currentStateCoroutine);
                }
                _currentState = State.PrepareNextAction;
                print("Preparing next action");
                _currentStateCoroutine = StartCoroutine(PrepareNextAction());
                break;
            case State.TrapPlayState:
                if (_currentStateCoroutine != null)
                {
                    StopCoroutine(_currentStateCoroutine);
                }
                print("Turning traps on and off");
                _currentState = State.TrapPlayState;
                _currentStateCoroutine = StartCoroutine(TrapFiring());
                break;
        }
    }

    public void FindPlayer()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _pC = GetPlayerController();
            if (_pC == null)
            {
                print("NULL");
            }
            if (_player == null)
            {
                Debug.Log("No gameobject in scene tagged with player");
            }
        }
    }
    public Card GetNextAction()
    {
        if (_actions.Count >= 1)
        {
            Card actionToGive = _actions[0];
            _actions.RemoveAt(0);
            return actionToGive;
        }
        return null;
    }
    public State GetState()
    {
        return _currentState;
    }
    public PlayerController GetPlayerController()
    {
        if (_pC != null)
        {
            return _pC;
        }
        return GetPlayer().GetComponent<PlayerController>();
    }
    public GameObject GetPlayer()
    {
        if (_player != null)
        {
            return _player;
        }
        {
            FindPlayer();
            return _player;
        }
    }

    public void HandleCardAdd(Card card)
    {
        AddCardToList(card);
    }
    public void HandleIncomingActions(List<Card> cardList)
    {
        StartCardActions(cardList);
    }
    public void HandleWallInterruption()
    {
        if (_pC.GetCurrentMovementCoroutine() != null)
        {
            _pC.StopCoroutine(_pC.GetCurrentMovementCoroutine());
            //_pC.StartFallCoroutine(transform.position, _pC.GetCurrentTile().GetPlayerSnapPosition());
        }
        FSM(State.PrepareNextAction);
    }
    public void HandleSpikeInterruption()
    {
        if (_pC.GetCurrentMovementCoroutine() != null)
        {
            _pC.StopCoroutine(_pC.GetCurrentMovementCoroutine());
        }
        FSM(State.WaitingForActions);
    }
    public void HandleReachedDestination()
    {
        if(_pC.GetCurrentMovementCoroutine() != null)
        {
            _pC.StopCoroutine(_pC.GetCurrentMovementCoroutine());
        }

        FSM(State.PrepareNextAction);
    }
    public void SetCardList(List<Card> incomingActions)
    {
        _actions.Clear();
        if(incomingActions != null)
        {
            _actions.AddRange(incomingActions);
        }
    }

    /// <summary>
    /// Adds a card to the 0th index
    /// </summary>
    /// <param name="card"></param>
    public void AddCardToList(Card card)
    {
        if(card != null)
        {
            _actions.Insert(0, card);
        }
        else
        {
            //Debug.LogError("Card is null");
        }
    }

    /// <summary>
    /// The method used to start a new action order from outside scripts
    /// </summary>
    /// <param name="incomingActions"></param>
    public void StartCardActions(List<Card> incomingActions)
    {
        _firedTraps = false;
        SetCardList(incomingActions);
        FSM(State.PrepareNextAction);
    }

    private IEnumerator PrepareNextAction()
    {
        while (_currentState == State.PrepareNextAction)
        {
            _currentAction = GetNextAction();
            if (_currentAction != null)
            {
                FSM(State.FindTileUponAction);
            }
            else if(!_firedTraps)
            {
                FSM(State.TrapPlayState);
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
        while (_currentState == State.FindTileUponAction)
        {
            var currentTile = _pC.GetCurrentTile();

            _distance = _currentAction.GetDistance(); //main focus of this state

            int facingDirection = _pC.GetCurrentFacingDirection();
            if (_pC.GetCurrentTile().GetObstacleClass() != null) //If youre standing on an obstacle that sends you in a direction
            {
               
                facingDirection = _pC.GetCurrentTile().GetObstacleClass().GetDirection();
                if(facingDirection == 4) //IF the tile doesnt have a facing direction, use the player's current FD
                {
                    facingDirection = _pC.GetCurrentFacingDirection();
                }
                if(_currentAction.name != Card.CardName.Jump)
                {
                    _pC.SetFacingDirection(facingDirection); //turn the player to face where they are going
                }               
            }

            _targetTile = TileManager.Instance.GetTileAtLocation(currentTile, facingDirection, _distance);

            FSM(State.PlayResult);
            yield return null;
        }
    }

    private IEnumerator PlayResult()
    {
        if (_currentState == State.PlayResult)
        {
            switch (_currentAction.name)
            {
                case Card.CardName.TurnLeft:
                    _pC.TurnPlayer(true);
                    PlayerController.ReachedDestination?.Invoke();
                    //TODO: listen for wait for turn player animation event 
                    break;
                case Card.CardName.TurnRight:
                    _pC.TurnPlayer(false);
                    PlayerController.ReachedDestination?.Invoke();
                    //TODO: animation here
                    break;
                case Card.CardName.Jump:
                    if (_distance > 1) //this is a spring tile
                    {
                        //_distance -= 1;
                        //uhhhhhh im counting on spring distance being three, because \/`8 = 2.8... almost 3 tiles.Code wise, i need it to be two
                        // (two up, two across) to work properly
                        //int[] possibleNumbers = { 0, 2, 6, 8 };
                        //int randomIndex = Random.Range(0, possibleNumbers.Length);

                    //    _targetTile = TileManager.Instance.GetTileAtLocation //TODO: must change from random to targetdirection or direction of targettile
                    //(_pC.GetCurrentTile(), possibleNumbers[randomIndex], _distance);

                        _pC.StartJumpCoroutine(_pC.GetCurrentTile().GetPlayerSnapPosition(), _targetTile.GetPlayerSnapPosition());
                    }

                    else // this is a normal jump
                    {
                        //determine result by getting difference of elevation betwen current tile and tile right in front of player
                        _distance += (_pC.GetCurrentTile().GetElevation() - 
                            (TileManager.Instance.GetTileAtLocation(_pC.GetCurrentTile(), _pC.GetCurrentFacingDirection(), 1).GetElevation()));
                        if (_distance < 0) //block is too high
                        {
                            Vector3 newVector = (_targetTile.GetPlayerSnapPosition());
                            _pC.StartJumpCoroutine(_pC.GetCurrentTile().GetPlayerSnapPosition(), new Vector3(newVector.x, newVector.y - 1, newVector.z));
                        }
                        else //block isnt too tall; need to add distance if 0 to actually jump onto the next block
                        {
                            if(_distance == 0)
                            {
                                _distance++;
                            }
                            _pC.StartJumpCoroutine(_pC.GetCurrentTile().GetPlayerSnapPosition(), 
                                TileManager.Instance.GetTileAtLocation(_pC.GetCurrentTile(), _pC.GetCurrentFacingDirection(), _distance).GetPlayerSnapPosition());
                        }
                    }
                    break;
                case Card.CardName.Move:
                    Vector3 newV = new Vector3(_targetTile.GetPlayerSnapPosition().x, _pC.transform.position.y, _targetTile.GetPlayerSnapPosition().z);
                    _pC.StartMoveCoroutine(_pC.GetCurrentTile().GetPlayerSnapPosition(), newV);
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitingForActions()
    {
        while (_currentState == State.WaitingForActions)
        {
            yield return null;
        }
    }

    private IEnumerator TrapFiring()
    {
        while(_currentState == State.TrapPlayState)
        {
          
            print("invoked");
            yield return new WaitForSeconds(1);
            GameManager.TrapAction?.Invoke();
            _firedTraps = true;
            if (_pC.GetTileWithPlayerRaycast().GetObstacleClass() != null)
            {
                AddCardToList(_pC.GetTileWithPlayerRaycast().GetObstacleClass().GetCard());
            }
            FSM(State.PrepareNextAction);
        }
    }

    private void OnDisable()
    {
        PlayerController.ReachedDestination -= HandleReachedDestination;
        PlayerController.AddCard -= HandleCardAdd;
        PlayerController.SpikeCollision -= HandleSpikeInterruption;
        PlayerController.WallInterruptAnimation -= HandleWallInterruption;
        GameManager.PlayActionOrder -= HandleIncomingActions;
    }
}
