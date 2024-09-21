// +-----------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last Modified - September 4st 2024
// @Description - The engine of the game which controls and initializes everything else
// +-----------------------------------------------------------------------------------+

using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Image _darken;
    [SerializeField] bool _doDebugMode;
    [SerializeField] private int _deathTimerLength;


    private DeckManager<Card> _deckManagerCard;
    private DeckManager<int> _deckManagerInt;
    private CardManager _cardManager;
    private UIManager _uiManager;

    public STATE gameState;
    private bool _gameWon;

    private LevelDeck _levelDeck;
    public List<Card> _deck;

    private List<int> _collectedSwitchIDs;
    private List<Collectable> collectablesCollected = new List<Collectable>();
    private (Card, int) _lastCardPlayed;
    public Card confirmationCard { get; private set; }
    public bool isSwitching, isClearing;
    private bool _isTurning;

    public bool _isTurningleft {get; private set; }
    #endregion

    public static Action<List<Card>> PlayActionOrder;
    public static Action<List<Card>> PlayDemoActionOrder;
    public static Action DeathAction;
    public static Action TrapAction;

    private bool _lowerDarkenIndex;
    private void Start()
    {
        //Carefully change order if needed. Some managers must be initialzed before others
        _deckManagerCard = DeckManager<Card>.Instance;
        _deckManagerInt = DeckManager<int>.Instance;
        _uiManager = UIManager.Instance;
        _cardManager = CardManager.Instance;
        _levelDeck = FindObjectOfType<LevelDeck>();
        _lowerDarkenIndex = false;

        isSwitching = false;
        _isTurning = false;
        isClearing = false;

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
            case STATE.ConfirmCards:
                gameState = STATE.ConfirmCards;
                PlayDemo();
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
                _gameWon = true;
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
        _darken.enabled = false;

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

        //If out of cards, go to corresponding game state AND IF NOT ALREADY WON
        if (_dealtCards.Count == 0 && _deck.Count == 0 && !_gameWon)
        {
            ChangeGameState(STATE.OutOfCards);
        }
        else if (!_gameWon)
        {
            _uiManager.UpdateTextBox("DRAG A CARD TO PLAY.");
            _uiManager.UpdateDealtCards(); //Updates Cards
        }
    }

    /// <summary>
    /// Runs the current action order
    /// Called after a card has been played
    /// </summary>
    private void RunPlaySequence()
    {
        _uiManager.UpdateConfirmCard();
        _uiManager.cancelButton.GetComponent<ConfirmationControls>().isActive = true;
        //If Clear Card was Played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Clear) //Error check and checks if last card played was a Clear
        {
            if (_playedCards.Count > 0)
            {
                _darken.enabled = true;
                isClearing = true;
                _uiManager.UpdateTextBox("SELECT A CARD TO CLEAR.");
            }
            else
            {
                confirmationCard = null;
                _uiManager.DestroyConfirmCard();
                ChangeGameState(STATE.ChooseCards);
            }    
        }


        //If Switch Card was played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Switch) //Error check and checks if last card played was a Switch
        {
            if (_playedCards.Count > 1)
            {
                _darken.enabled = true;
                isSwitching = true;
                _uiManager.UpdateTextBox("SELECT TWO CARDS TO SWAP.");
            }
            else
            {
                confirmationCard = null;
                _uiManager.DestroyConfirmCard();
                ChangeGameState(STATE.ChooseCards);
            }
        }

        //If Turn Card was played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Turn) //Error check and checks if last card played was a Switch
        {
            _darken.enabled = true;
            _darken.transform.SetSiblingIndex(_darken.transform.GetSiblingIndex() + 1);
            _lowerDarkenIndex = true;
            ChangeGameState(STATE.ChooseTurn); //Waits for User Input to Switch two cards
            _uiManager.UpdateTextBox("CHOOSE TO TURN LEFT OR RIGHT.");
            _uiManager.CreateTurnCards();
            _isTurning = true;
        }
        if (!isClearing && !isSwitching && !_isTurning && gameState != STATE.ChooseCards)
        {
            _uiManager.confirmButton.GetComponent<ConfirmationControls>().isActive = true;
            ChangeGameState(STATE.ConfirmCards);
        }
    }

    /// <summary>
    /// Plays a forecast on where the player will move
    /// </summary>
    private void PlayDemo()
    {
        List<Card> tempList = new();

        //Adds the confirmation card to be played in demo
        int tempSize = tempList.Count;
        for (int i = 0; i < tempSize; i++)
        {
            tempList.Add(_playedCards[i]);
        }
        tempList.Add(confirmationCard);

        PlayDemoActionOrder?.Invoke(tempList);
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
        ChangeGameState(STATE.Death);
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
                confirmationCard = _lastCardPlayed.Item1;
                break;
            }
        }
        ChangeGameState(STATE.RunActionOrder);
    }

    /// <summary>
    /// If the confirm button is pressed, 
    /// </summary>
    public void ConfirmCards()
    {
        //NOTE:
        //If Clear and Switch do not want to play animation, check if _isClearing and _isSwitching is false
        //If they are both false, do _uiManager.MoveCardToActionOrder() AND do the following at end of method
        // _uiManager.UpdatePlayedCards();
        // _uiManager.UpdateDealtCards();
        // PlaySequence();

        //Removes all highlight from cards
        List<Image> tempPlayedCards = _uiManager.GetInstantiatedPlayedCardImages();

        int tempCount = tempPlayedCards.Count;
        for (int i = 0; i < tempCount; i++)
        {
            tempPlayedCards[i].gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
            tempPlayedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
        }

        _uiManager.MoveCardToActionOrder();
        _uiManager.DisableTextBox();
        //If the confirmation card is a clear or switch, do not add it into play order
        if (confirmationCard.name != Card.CardName.Clear && confirmationCard.name != Card.CardName.Switch)
            _playedCards.Add(confirmationCard);

        _dealtCards.Remove(_lastCardPlayed.Item1);

        //If the confirmed card was a clear card
        if (isClearing)
        {
            isClearing = false;

            //sound effect call
            SfxManager.Instance.PlaySFX(6189);

            List<Image> instantiatedImages = _uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

            int instantiatedImagesCount = instantiatedImages.Count;
            for (int i = 0; i < instantiatedImagesCount; i++)
            {
                print(_cardManager.clearCard);
                if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == _cardManager.clearCard.GetComponentInChildren<CardDisplay>().ID) //Compares instantiated images' unique ID to the target ID
                {
                    _playedCards = _deckManagerCard.RemoveAt(_playedCards, i); //When IDs match, remove the card from the list
                    break;
                }
            }
            _cardManager.clearCard = null;
        }

        //If the confirmed card was a switch card
        if (isSwitching)
        {
            isSwitching = false;

            List<Image> instantiatedImages = _uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

            int instantiatedImagesCount = instantiatedImages.Count;

            //Initializes variables
            int target1Index = -1;
            int target2Index = -1;
            (Image, Image) switchCards = _cardManager.switchCards;

            //Finds the first target ID
            for (int i = 0; i < instantiatedImagesCount; i++)
            {
                if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == switchCards.Item1.GetComponentInChildren<CardDisplay>().ID) //Compares instantiated images' unique ID to the target ID
                {
                    target1Index = i;

                    break;
                }
            }

            //Finds the second target ID
            for (int i = 0; i < instantiatedImagesCount; i++)
            {
                if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == switchCards.Item2.GetComponentInChildren<CardDisplay>().ID) //Compares instantiated images' unique ID to the target ID
                {
                    target2Index = i;

                    break;
                }
            }
            if (target1Index != -1 && target2Index != -1) //Error check. Does not continue if both cards are not found
            {
                _playedCards = _deckManagerCard.Swap(_playedCards, target1Index, target2Index); //Swaps the two cards
            }
            else
            {
                print("FAILED TO LOCATE CARD IDS");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void CancelCard()
    {
        //Removes all highlight from the images
        List<Image> tempPlayedCards = _uiManager.GetInstantiatedPlayedCardImages();
        _cardManager.clearCard = null;
        _cardManager.switchCards.Item1 = null;
        _cardManager.switchCards.Item2 = null;
        _uiManager.DestroyTurnCards();

        int tempCount = tempPlayedCards.Count;
        for (int i = 0; i < tempCount; i++)
        {
            tempPlayedCards[i].gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
            tempPlayedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
        }

        //Resets to prior state
        if (_lowerDarkenIndex)
        {
            _darken.transform.SetSiblingIndex(_darken.transform.GetSiblingIndex() - 1);
            _lowerDarkenIndex = false;
        }
        _darken.enabled = false;
        isClearing = false;
        isSwitching = false;

        _cardManager.RemoveAllHighlight(_uiManager.GetInstantiatedPlayedCardImages()); //Removes the highlight
        _collectedSwitchIDs = new(); //Clears the list
        ChangeGameState(STATE.ChooseCards);
    }

    /// <summary>
    /// Checks the sequence and calls the animation based off the card
    /// Called after the player chooses a card and when the player returns to the idle animation
    /// </summary>
    public void PlaySequence()
    {
        if (_lowerDarkenIndex)
        {
            _darken.transform.SetSiblingIndex(_darken.transform.GetSiblingIndex() - 1);
            _lowerDarkenIndex = false;
        }
        _darken.enabled = false;
        //Invokes Action that Eli's script is listening to
        PlayActionOrder?.Invoke(_playedCards);

        if (_doDebugMode)
            ChangeGameState(STATE.ChooseCards);
    }

    /// <summary>
    /// Adds a card at the bottom of the action order
    /// </summary>
    /// <param name="card">The Card to add</param>
    public void AddTurnCard(Card card, bool isTurningLeft)
    {
        confirmationCard = card;
        _isTurning = false;
        _isTurningleft = isTurningLeft;
        ChangeGameState(STATE.RunActionOrder);
    }

    #region Action Order Card Effects
    /// <summary>
    /// Functionality of the Clear Card
    /// </summary>
    public void ClearAction()
    {
        _uiManager.UpdateConfirmCard();
        ChangeGameState(STATE.ConfirmCards);
    }

    /// <summary>
    /// Functionality of the Switch Card
    /// </summary>
    /// <param name="firstTargetID">The first card ID to switch</param>
    /// <param name="secondTargetID">The second card ID to switch</param>
    public void SwitchAction()
    {
        (Image, Image) cards = _cardManager.switchCards;

        if (cards.Item1 != null && cards.Item2 != null)
        {
            _uiManager.UpdateConfirmCard();
            ChangeGameState(STATE.ConfirmCards);
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
        SceneManager.LoadScene(2);
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
    /// Gets the last played card
    /// </summary>
    /// <returns>A Card - the last card played, waiting to be confirmed</returns>
    public Card GetLastPlayedCard() { return confirmationCard; }

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
        ConfirmCards,
        Trap,
        ChooseTurn,
        Death,
        OutOfCards,
        End
    }
}