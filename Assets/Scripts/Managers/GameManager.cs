// +-----------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last Modified - September 4st 2024
// @Description - The engine of the game which controls and initializes everything else
// +-----------------------------------------------------------------------------------+

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Makes Class a Singleton Class.
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(GameManager)) as GameManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion
    #region Variables
    [SerializeField] private List<Card> _dealtCards;
    [SerializeField] private List<Card> _playedCards;
    [SerializeField] bool _doDebugMode;
    [SerializeField] private int _deathTimerLength;


    private DeckManager<Card> _deckManagerCard;
    private DeckManager<int> _deckManagerInt;
    private CardManager _cardManager;
    private UIManager _uiManager;

    public STATE gameState;

    private LevelDeck _levelDeck;
    private List<Card> _deck;

    private List<Card> _tempPlayedCards;
    private List<Card> _tempBeforeBackToItCards, _tempAfterBackToItCards;
    private List<int> _collectedSwitchIDs;
    private List<Collectable> collectablesCollected = new List<Collectable>();
    private (Card, int) _lastCardPlayed;
    private int _lastBackToItIndex;
    #endregion

    public static Action<List<Card>> PlayActionOrder;
    public static Action DeathAction;
    public static Action TrapAction;
    private void Start()
    {
        //Carefully change order if needed. Some managers must be initialzed before others
        _deckManagerCard = DeckManager<Card>.Instance;
        _deckManagerInt = DeckManager<int>.Instance;
        _uiManager = UIManager.Instance;
        _cardManager = CardManager.Instance;
        _levelDeck = FindObjectOfType<LevelDeck>();
        _lastBackToItIndex = -1;

        ChangeGameState(STATE.LoadGame);
    }

    public void OnEnable()
    {
        DeathAction += DeathMethod;
    }

    public void OnDisable()
    {
        DeathAction -= DeathMethod;
    }

    /// <summary>
    /// Method that is listening to the DeathAction being invoked.
    /// </summary>
    private void DeathMethod()
    {
        //print("Rip Bozo");
        //_levelDeck.ResetDeck();

        //_deck = _levelDeck.deck;
        //_dealtCards = new();
        //_playedCards = new();
        //_tempPlayedCards = new();
        //_tempBeforeBackToItCards = new();
        //_tempAfterBackToItCards = new();
        //_collectedSwitchIDs = new();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Controls the current game state.
    /// 
    /// Always use gameState = STATE.[State] for every branch or game will break
    /// </summary>
    public void ChangeGameState(STATE state)
    {
        //Checks for what state to change to
        switch (state)
        {
            case STATE.LoadGame:
                //Sets up and initializes necessary functions to play
                gameState = STATE.LoadGame; //call when level loads
                StartGame();
                break;
            case STATE.Menu:
                //Menu methods called
                gameState = STATE.Menu;
                break;
            case STATE.StartLevel:
                gameState = STATE.StartLevel;
                SetUpLevel();
                break;
            case STATE.ChooseCards:
                //Choose Cards methods called
                gameState = STATE.ChooseCards;
                DealCards();
                break;
            case STATE.RunActionOrder:
                gameState = STATE.RunActionOrder;
                RunPlaySequence();
                break;
            case STATE.ChooseClear:
                // Waiting STATE. Game locks in this state until user input
                gameState = STATE.ChooseClear;
                break;
            case STATE.SwitchCards:
                // Waiting STATE. Game locks in this state until user input
                gameState = STATE.SwitchCards;
                break;
            case STATE.ChooseTurn:
                // Waiting STATE. Game locks in this state until user input
                gameState = STATE.ChooseTurn;
                break;
            case STATE.Death:
                gameState = STATE.Death;
                Failure();
                break;
            case STATE.OutOfCards:
                gameState = STATE.OutOfCards;
                OutOfCards();
                break;
            case STATE.End:
                gameState = STATE.End;
                //Add method here if needed
                Invoke("LoadLevelSelect", 1);
                break;
            default:
                //Error check
                print("ERROR: FAILED TO SWITCH GAME STATE.");
                break;
        }
    }

    #region State Machine Methods
    /// <summary>
    /// Initializes everything needed for the game
    /// Called at the start of the game
    /// </summary>
    private void StartGame()
    {
        //Initialzes other managers. Order may matter
        _uiManager.Init();
        _cardManager.Init();
        _levelDeck.Init();

        //Initializes lists.
        _deck = new();
        _dealtCards = new();
        _playedCards = new();
        _tempPlayedCards = new();
        _tempBeforeBackToItCards = new();
        _tempAfterBackToItCards = new();

        _collectedSwitchIDs = new();

        ChangeGameState(STATE.StartLevel);
    }

    /// <summary>
    /// Sets up the current level
    /// Called when level is selected
    /// </summary>
    private void SetUpLevel()
    {
        _deck = _levelDeck.deck;

        //Add whatever additional set up here (after clicking on a level from the level to the point the player can start choosing cards)

        ChangeGameState(STATE.ChooseCards);
    }

    /// <summary>
    /// Gets how many dealt cards there are and draws up to max hand size (4)
    /// Called at the beginning of every turn
    /// </summary>
    private void DealCards()
    {
        //Draws until the player has 4 cards or until the deck runs out
        while (_dealtCards.Count < 4 && _deck.Count > 0)
        {
            if (_lastCardPlayed.Item1 != null)
            {
                //Replaces the last played card with a new card in the same index
                //Keeps the other dealt cards in the same location
                _dealtCards.Insert(_lastCardPlayed.Item2, _deck[0]);
            }
            else
            {
                //Adds the top card from the deck onto the dealtCards
                _dealtCards.Add(_deck[0]);
            }
            _deck = _deckManagerCard.RemoveFirst(_deck); //Removes the now dealt card from the deck
        }

        //If out of cards, go to corresponding game state
        if (_dealtCards.Count == 0 && _deck.Count == 0)
        {
            ChangeGameState(STATE.OutOfCards);
        }
        _uiManager.UpdateDealtCards(); //Updates Cards
    }

    /// <summary>
    /// Runs the current action order
    /// Called after a card has been played
    /// </summary>
    private void RunPlaySequence()
    {
        _lastBackToItIndex = -1;

        _tempPlayedCards.Clear(); //Clears temporary action order

        int playedCardsCount = _playedCards.Count;

        //Adds copies of the cards into a temporary list
        for (int i = 0; i < playedCardsCount; i++)
        {
            _tempPlayedCards.Add(_playedCards[i]);
        }

        //If Clear Card was Played
        if (_playedCards.Count > 0 && _playedCards[_playedCards.Count - 1].name == Card.CardName.Clear) //Error check and checks if last card played was a Clear
        {
            _playedCards = _deckManagerCard.RemoveLast(_playedCards); //Removes clear card from played carsds
            _uiManager.UpdatePlayedCards(); //Updates played cards so clear card does not appear
            if (_playedCards.Count > 0) //If there is a card to clear, call ClearAction
            {
                ChangeGameState(STATE.ChooseClear); //Waits for User Input to Clear a Card
            }
        }

        //If Switch Card was played
        if (_playedCards.Count > 0 && _playedCards[_playedCards.Count - 1].name == Card.CardName.Switch) //Error check and checks if last card played was a Switch
        {
            _playedCards = _deckManagerCard.RemoveLast(_playedCards); //Removes switch card from played cards
            _uiManager.UpdatePlayedCards(); //Updates played cards so switch card does not appear
            if (_playedCards.Count > 1) //Performs switch if there are at least two cards in the deck
            {
                ChangeGameState(STATE.SwitchCards); //Waits for User Input to Switch two cards
            }
        }

        //If Turn Card was played
        if (_playedCards.Count > 0 && _playedCards[_playedCards.Count - 1].name == Card.CardName.Turn) //Error check and checks if last card played was a Switch
        {
            _playedCards = _deckManagerCard.RemoveLast(_playedCards); //Removes switch card from played cards
            _uiManager.UpdatePlayedCards(); //Updates played cards so switch card does not appear
            ChangeGameState(STATE.ChooseTurn); //Waits for User Input to Switch two cards
            _uiManager.CreateTurnCards();
        }
        if (gameState == STATE.RunActionOrder)
            PlaySequence(); //Plays the action order
    }

    /// <summary>
    /// Toggles traps on and off
    /// </summary>
    public void ToggleTraps()
    {
        TrapAction?.Invoke();
    }

    /// <summary>
    /// When the player fails, this method is called
    /// </summary>
    private void Failure()
    {
        DeathAction?.Invoke();
        StartCoroutine(DeathTimer());
    }

    /// <summary>
    /// When the player runs out of cards, this method is called
    /// </summary>
    private void OutOfCards()
    {
        print("Skill Issue."); //TODO - Replace this with actual functionality
    }

    #endregion

    /// <summary>
    /// Removes a card from the dealt deck, and adds it to the played deck
    /// </summary>
    /// <param name="targetID">The ID of the card to play</param>
    public void PlayCard(int targetID)
    {
        //Due to the Cards being Scriptable Objects, I couldn't find a good way to give them unique IDs
        //So I gave the images unique IDs and used those. Weird way, but it works

        List<Image> instantiatedImages = _uiManager.GetInstantiatedDealtCardImages(); //Gets instantiated dealt card images

        int instantiatedImagesCount = instantiatedImages.Count;
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == targetID) //Compares instantiated images' unique ID to the target ID
            {
                Card playedCard = _dealtCards[i]; //Gets the same ID card
                _lastCardPlayed = (playedCard, i); //Stores card and the index
                _dealtCards = _deckManagerCard.RemoveAt(_dealtCards, i); //Removes card from the dealt deck

                _playedCards.Add(playedCard); //Adds card to the played deck

                break;
            }
        }

        _uiManager.UpdatePlayedCards();
        ChangeGameState(STATE.RunActionOrder);
    }

    /// <summary>
    /// Checks the sequence and calls the animation based off the card
    /// Called after the player chooses a card and when the player returns to the idle animation
    /// </summary>
    public void PlaySequence()
    {
        //Invokes Action that Eli's script is listening to
        PlayActionOrder?.Invoke(_playedCards);

        if (_doDebugMode)
            ChangeGameState(STATE.ChooseCards);
    }

    /// <summary>
    /// Adds a card at the bottom of the action order
    /// </summary>
    /// <param name="card">The Card to add</param>
    public void AddToPlayedCards(Card card)
    {
        _playedCards.Add(card);
        _uiManager.UpdatePlayedCards();
        ChangeGameState(STATE.RunActionOrder);
    }

    #region Action Order Card Effects
    /// <summary>
    /// Functionality of the Clear Card
    /// </summary>
    public void ClearAction(int targetID)
    {
        //Due to the Cards being Scriptable Objects, I couldn't find a good way to give them unique IDs
        //So I gave the images unique IDs and used those. Weird way, but it works

        List<Image> instantiatedImages = _uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

        int instantiatedImagesCount = instantiatedImages.Count;
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == targetID) //Compares instantiated images' unique ID to the target ID
            {
                _playedCards = _deckManagerCard.RemoveAt(_playedCards, i); //When IDs match, remove the card from the list
                break;
            }
        }
        
        _uiManager.UpdatePlayedCards();
        ChangeGameState(STATE.RunActionOrder);
    }

    /// <summary>
    /// Functionality of the Switch Card
    /// </summary>
    /// <param name="firstTargetID">The first card ID to switch</param>
    /// <param name="secondTargetID">The second card ID to switch</param>
    private void SwitchAction(int firstTargetID, int secondTargetID)
    {
        //Due to the Cards being Scriptable Objects, I couldn't find a good way to give them unique IDs
        //So I gave the images unique IDs and used those. Weird way, but it works

        List<Image> instantiatedImages = _uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

        int instantiatedImagesCount = instantiatedImages.Count;

        //Initializes variables
        int target1Index = -1;
        int target2Index = -1;

        //Finds the first target ID
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == firstTargetID) //Compares instantiated images' unique ID to the target ID
            {
                target1Index = i;

                break;
            }
        }

        //Finds the second target ID
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == secondTargetID) //Compares instantiated images' unique ID to the target ID
            {
                target2Index = i;

                break;
            }
        }
        if (target1Index != -1 && target2Index != -1) //Error check. Does not continue if both cards are not found
        {
            _playedCards = _deckManagerCard.Swap(_playedCards, target1Index, target2Index); //Swaps the two cards
        }
        
        _uiManager.UpdatePlayedCards();

        ChangeGameState(STATE.RunActionOrder);
    }

    //Switch List Variable Initialization


    /// <summary>
    /// Receives user input and does checks until conditions are met to continue
    /// </summary>
    /// <param name="targetID">The ID of one of the cards to switch</param>
    public void SwitchActionHelper(int targetID)
    {
        List<Image> instantiatedImages = _uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

        int instantiatedImagesCount = instantiatedImages.Count;
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == targetID) //Compares instantiated images' unique ID to the target ID
            {
                int collectedIdsListCount = _collectedSwitchIDs.Count;

                //If the list contains the target ID already
                if (_collectedSwitchIDs.Contains(targetID))
                {
                    //There can only be two Cards in the list. If there is a duplicate ID, it must be the first card
                    _collectedSwitchIDs = _deckManagerInt.RemoveFirst(_collectedSwitchIDs); 
                }
                //If the list does not contain the target ID
                else
                {
                    _collectedSwitchIDs.Add(targetID);
                }
                
                break;
            }
        }

        //If two cards have been selected, perform switch action
        if (_collectedSwitchIDs.Count == 2)
            SwitchAction(_collectedSwitchIDs[0], _collectedSwitchIDs[1]);
    }

    /// <summary>
    /// Functionality of the Back To It Card
    /// </summary>
    private void BackToItAction()
    {
        _tempBeforeBackToItCards.Clear();
        _tempAfterBackToItCards.Clear();
        //Gets index of BackToIt card
        int playedCardsSize = _playedCards.Count;

        //If this is first instance of BackToIt in playedCards
        if (_lastBackToItIndex == -1)
        {
            //Gets the first instance of BackToIt's index in playedCards
            for (int i = 0; i < playedCardsSize; i++)
            {
                if (_playedCards[i].name == Card.CardName.BackToIt)
                {
                    _lastBackToItIndex = i;
                    break;
                }
            }
        }
        //If this is the second or greater instance of BackToIt in playedCards
        else
        {
            //Gets the next instance of BackToIt's index in playedCards
            for (int i = _lastBackToItIndex + 1; i < playedCardsSize; i++)
            {
                if (_playedCards[i].name == Card.CardName.BackToIt)
                {
                    _lastBackToItIndex = i;
                    break;
                }
            }
        }

        //Copies all cards before BackToIt Card
        for (int i = 0; i < _lastBackToItIndex; i++)
        {
            //Ignores previous Back To It Cards
            if (_playedCards[i].name != Card.CardName.BackToIt)
            {
                _tempBeforeBackToItCards.Add(_playedCards[i]);
            }
        }
        //Copies all cards after BackToIt Card
        for (int i = _lastBackToItIndex + 1; i < playedCardsSize; i++)
        {
            _tempAfterBackToItCards.Add(_playedCards[i]);
        }

        //Clears the tempPlayedCardsList
        _tempPlayedCards.Clear();

        //Adds card to be removed at front of list ( due to RemoveAt(0) at the end of PlaySequence() )
        _tempPlayedCards.Add(_playedCards[_lastBackToItIndex]);

        //Adds all cards before Back To It into the list
        int beforeBackToItSize = _tempBeforeBackToItCards.Count;
        for (int i = 0; i < beforeBackToItSize; i++)
        {
            _tempPlayedCards.Add(_tempBeforeBackToItCards[i]);
        }

        //Adds all cards after Back To It into the list
        int afterBackToItSize = _tempAfterBackToItCards.Count;
        for (int i = 0; i < afterBackToItSize; i++)
        {
            _tempPlayedCards.Add(_tempAfterBackToItCards[i]);
        }
    }
    #endregion

    /// <summary>
    /// Starts a new turn
    /// </summary>
    public void NewTurn()
    {
        ChangeGameState(STATE.ChooseCards);
    }


    public void AddCollectable(Collectable collectable)
    {
        collectablesCollected.Add(collectable);
        _uiManager.UpdateCollectables();
    }

    /// <summary>
    /// Timer to reset the game state
    /// </summary>
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(_deathTimerLength);
        ChangeGameState(STATE.StartLevel);
        yield return null;
    }

    private void LoadLevelSelect()
    {
        SceneManager.LoadScene(0);
    }

    #region Getters
    public int GetCollectableCount() { return collectablesCollected.Count; }

    /// <summary>
    /// Gets the current dealt cards
    /// </summary>
    /// <returns>A List<Card> Cards that have been dealt</returns>
    public List<Card> GetDealtCards() { return _dealtCards; }


    /// <summary>
    /// Gets the current played cards
    /// </summary>
    /// <returns>A List<Card> Cards that have been played</returns>
    public List<Card> GetPlayedCards() { return _playedCards; }

    /// <summary>
    /// Gets the current collected switch IDs
    /// </summary>
    /// <returns>List<int> list of IDs from played cards being selected to be swapped</returns>
    public List<int> GetCollectedSwitchIDs() { return _collectedSwitchIDs; }
    #endregion

    public enum STATE
    {
        LoadGame,
        Menu,
        StartLevel,
        ChooseCards,
        RunActionOrder,
        ChooseClear,
        SwitchCards,
        Trap,
        ChooseTurn,
        Death,
        OutOfCards,
        End
    }
}