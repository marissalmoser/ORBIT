// +-----------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - Elijah Vroman
// @Last Modified - November 21st 2024
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
    [SerializeField] private float _deathTimerLength;
    [SerializeField] private Texture2D _clearCursor;
    [SerializeField] private Texture2D _switchCursor;
    public String currentCursor;
    private Vector2 _clearCursorHotspot;
    private Vector2 _switchCursorHotspot;
    private Vector2 _sunnnyCursorHotspot = new Vector2(16, 16);

    public Image darken;
    public Image deckShownDarken;
    [NonSerialized] public bool isConfirmCardThere;

    private DeckManager<Card> _deckManagerCard;
    private DeckManager<int> _deckManagerInt;
    private CardManager _cardManager;
    private UIManager _uiManager;
    private CollectibleStats _collectibleStats;
    private ButtonsManager _buttonsManager;

    public STATE gameState;
    private bool _gameWon, _gameLost;
    private bool _collectableCollected;

    private LevelDeck _levelDeck;
    public List<Card> deck;
    private List<Card> _demoDeck;

    private List<int> _collectedSwitchIDs;
    private List<Collectable> collectablesCollected = new List<Collectable>();
    public (Card, int) lastCardPlayed;
    public Card confirmationCard;
    [NonSerialized] public bool isSwitching, isClearing, isStalling, isUsingWild, currentlyOnWild;
    [NonSerialized] public bool currentlyOnTurn, isTurning;

    [NonSerialized] public List<Card> _startingDeck;

    public bool isTurningleft {get; private set; }
    private bool _getOriginalDeck;
    public bool hasSwitched { get; private set; }
    private bool _shouldMoveDealtCards;
    #endregion
    #region Actions
    public static Action<List<Card>> PlayActionOrder;
    public static Action<List<Card>> PlayDemoActionOrder;
    public static Action DeathAction;
    public static Action TrapAction;
    public static Action WinAction;
    #endregion

    [NonSerialized] public bool lowerDarkenIndex;
    private void Start()
    {
        Application.targetFrameRate = 60;
        //Carefully change order if needed. Some managers must be initialzed before others
        _deckManagerCard = DeckManager<Card>.Instance;
        _deckManagerInt = DeckManager<int>.Instance;
        _uiManager = UIManager.Instance;
        _cardManager = CardManager.Instance;
        _buttonsManager = ButtonsManager.Instance;
        _levelDeck = FindObjectOfType<LevelDeck>();
        lowerDarkenIndex = false;

        isSwitching = false;
        currentlyOnTurn = false;
        isTurning = false;
        isClearing = false;
        isStalling = false;
        isUsingWild = false;
        currentlyOnWild = false;
        hasSwitched = false;
        _getOriginalDeck = true;
        isConfirmCardThere = false;
        _shouldMoveDealtCards = false;

        if (_clearCursor != null)
        {
            _clearCursorHotspot = new Vector2(_clearCursor.width / 2, _clearCursor.height / 2);
        }
        if (_switchCursor != null)
        {
            _switchCursorHotspot = new Vector2(_switchCursor.width / 2, _switchCursor.height / 2);
        }
        ChangeGameState(STATE.LoadGame);
    }

    public void EnableGM()
    {
        print("in gm");
        //Carefully change order if needed. Some managers must be initialzed before others
        _deckManagerCard = DeckManager<Card>.Instance;
        _deckManagerInt = DeckManager<int>.Instance;
        _uiManager = UIManager.Instance;
        _cardManager = CardManager.Instance;
        _buttonsManager = ButtonsManager.Instance;
        _levelDeck = FindObjectOfType<LevelDeck>();
        lowerDarkenIndex = false;

        isSwitching = false;
        currentlyOnTurn = false;
        isTurning = false;
        isClearing = false;
        isStalling = false;
        isUsingWild = false;
        currentlyOnWild = false;
        hasSwitched = false;
        _getOriginalDeck = true;
        isConfirmCardThere = false;

        if (_clearCursor != null)
        {
            _clearCursorHotspot = new Vector2(_clearCursor.width / 2, _clearCursor.height / 2);
        }
        if (_switchCursor != null)
        {
            _switchCursorHotspot = new Vector2(_switchCursor.width / 2, _switchCursor.height / 2);
        }
        ChangeGameState(STATE.LoadGame);
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
            case STATE.ConfirmCards:
                gameState = STATE.ConfirmCards;
                PlayDemo();
                break;
            case STATE.PlayingActionOrder:
                gameState = STATE.PlayingActionOrder;
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
                if(_collectableCollected)
                {
                    CollectibleManager.Instance.CollectCollectible();
                }
                StartCoroutine(OnWinDelay());
                //Invoke("LoadLevelSelect", 1);
                break;
            default:
                //Error check
                print("ERROR: FAILED TO SWITCH GAME STATE.");
                break;
        }
    }

    IEnumerator OnWinDelay()
    {
        yield return new WaitForSeconds(1);
        WinAction?.Invoke();
        LoadLevelSelect();
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
        deck = new();
        _demoDeck = new();
        _startingDeck = new();
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
        //Gets the deck
        deck = _levelDeck.deck;
        darken.enabled = false;

        //Keeps permenent record of the original deck
        foreach (var card in deck)
        {
            _startingDeck.Add(card);
        }

        //Disables Darken Effects
        darken.enabled = false;
        deckShownDarken.enabled = false;

        //if no pop up menu in level, start player falling coroutine. The contine button on pop up menus has the same functionality.
        if (FindObjectOfType<PopUpMenu>() == null)
        {
            TileManager.Instance.StartCoroutine(TileManager.Instance.FallAllGameObjects());
        }
        //Tilemanager 249 calls choose cards now after game start
    }

    /// <summary>
    /// Gets how many dealt cards there are and draws up to max hand size (4)
    /// Called at the beginning of every turn
    /// </summary>
    private void DealCards()
    {
        CardManager.Instance.canMoveCard = false;
        List<(Card, int)> newDealtCards = new();
        //Draws until the player has 4 cards or until the deck runs out
        while (_dealtCards.Count < 4 && deck.Count > 0)
        {
            if (lastCardPlayed.Item1 != null)
            {
                //Replaces the last played card with a new card in the same index
                //Keeps the other dealt cards in the same location
                _dealtCards.Insert(lastCardPlayed.Item2, deck[0]);
                newDealtCards.Add((deck[0], lastCardPlayed.Item2));
            }
            else
            {
                //Adds the top card from the deck onto the dealtCards
                newDealtCards.Add((deck[0], _dealtCards.Count));
                _dealtCards.Add(deck[0]);
            }

            deck = _deckManagerCard.RemoveFirst(deck); //Removes the now dealt card from the deck
        }

        //If out of cards, go to corresponding game state AND IF NOT ALREADY WON
        if (_dealtCards.Count == 0 && deck.Count == 0 && !_gameWon)
        {
            ChangeGameState(STATE.OutOfCards);
        }
        else if (!_gameWon)
        {
            for (int i = 0; i < newDealtCards.Count; i++) //Shows animation for cards
            {
                _uiManager.StartMoveCardFromDeck(newDealtCards[i].Item1, newDealtCards[i].Item2, newDealtCards.Count);
            }
            if (newDealtCards.Count == 0) //If a new turn is activated and no cards are dealt, continue the game as normal
            {
                if (_shouldMoveDealtCards) //If a card was actually removed from the dealt cards
                {
                    List<Image> dealtImages = _uiManager.GetInstantiatedDealtCardImages();
                    dealtImages.RemoveAt(lastCardPlayed.Item2);
                    int imageCount = dealtImages.Count;
                    for (int i = 0; i < imageCount; i++)
                    {
                        _uiManager.StartMoveCard(dealtImages[i], i);
                    }
                }
                else // If DealCards() was called but no cards were removed from the deck (for example, when the cancel button is pressed)
                {
                    _uiManager.UpdateTextBox("DRAG A CARD TO PLAY.");
                    _uiManager.UpdateDealtCards(_dealtCards); //Updates Cards
                    CardManager.Instance.canMoveCard = true;
                }
            }
        }
    }

    /// <summary>
    /// Runs the current action order
    /// Called after a card has been played
    /// </summary>
    public void RunPlaySequence()
    {
        _uiManager.UpdateConfirmCard();
        _uiManager.cancelButton.GetComponent<ButtonControls>().SetIsActive(true);
        //If Clear Card was Played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Clear) //Error check and checks if last card played was a Clear
        {
            if (_playedCards.Count > 0)
            {
                darken.enabled = true;
                isClearing = true;
                _uiManager.UpdateTextBox("SELECT A CARD TO CLEAR.");

                if (_clearCursor != null)
                {
                    SetCursor("Clear");
                }
            }
            else
            {
                confirmationCard = null;
                _uiManager.DestroyConfirmCard();
            }
        }

        //If Switch Card was played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Switch) //Error check and checks if last card played was a Switch
        {
            if (_playedCards.Count > 1)
            {
                darken.enabled = true;
                isSwitching = true;
                _uiManager.UpdateTextBox("SELECT TWO CARDS TO SWAP.");

                if (_switchCursor != null)
                {
                    SetCursor("Switch");
                } 
            }

            else
            {
                confirmationCard = null;
                _uiManager.DestroyConfirmCard();

            }
        }

        //If Turn Card was played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Turn) //Error check and checks if last card played was a Switch
        {
            darken.enabled = true;
            darken.transform.SetSiblingIndex(darken.transform.GetSiblingIndex() + 1);
            lowerDarkenIndex = true;
            ChangeGameState(STATE.ChooseTurn); //Waits for User Input to Switch two cards
            _uiManager.UpdateTextBox("CHOOSE TO TURN LEFT OR RIGHT.");
            currentlyOnTurn = true;
            isTurning = true;
            _buttonsManager.ChangeMaxIndex(2);
            _buttonsManager.ResetIndex();
        }
        //If Stall Card was played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Stall) //Error check and checks if last card played was a Stall
        {
            isStalling = true;
        }

        //If Wild Card was played
        if (confirmationCard != null && confirmationCard.name == Card.CardName.Wild) //Error check and checks if last card played was a Stall
        {
            darken.enabled = true;
            darken.transform.SetSiblingIndex(darken.transform.GetSiblingIndex() + 1);
            lowerDarkenIndex = true;
            isUsingWild = true;
            currentlyOnWild = true;
            _buttonsManager.ChangeMaxIndex(_uiManager.numOfUniqueCards);
            _buttonsManager.ResetIndex();
        }

        //Updates the arrows
        _uiManager.UpdateArrows();

        //If demo is good to go
        if (!isClearing && !isSwitching && !currentlyOnTurn && !currentlyOnWild)
        {
            isConfirmCardThere = true;
            _uiManager.confirmButton.GetComponent<ButtonControls>().SetIsActive(true);
            ChangeGameState(STATE.ConfirmCards);
        }
    }

    /// <summary>
    /// Plays a forecast on where the player will move
    /// </summary>
    /// <param name="editedList">The tempList that was edited. If it was not edited, give the original List</param>
    /// <param name="originalList">The original List</param>
    private void PlayDemo()
    {
        _uiManager.UpdateTextBox("Confirm / Cancel");

        //Copies played cards to the demo deck
        //Restores deck when clearing to avoid losing cards
        if (_getOriginalDeck || isClearing)
        {
            _demoDeck = new();
            int playedCardsCount = _playedCards.Count;
            for (int i = 0; i < playedCardsCount; i++)
            {
                _demoDeck.Add(_playedCards[i]);
            }

            //Adds the confirmation card to be played in demo
            if (!isClearing && !isSwitching && !isStalling && !currentlyOnWild)
                _demoDeck.Add(confirmationCard);

            _getOriginalDeck = false;
        }

        //If the confirmed card was a clear card
        if (isClearing)
        {
            //sound effect call
            SfxManager.Instance.PlaySFX(6189);

            List<Image> instantiatedImages = _uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

            int instantiatedImagesCount = instantiatedImages.Count;

            int numOfClearedCards = 0;
            for (int i = 0; i < instantiatedImagesCount; i++)
            {
                //Loops over clear list
                for (int j = 0; j < _cardManager.numOfCardsToClear; j++)
                {
                    //Compares instantiated images' unique ID to the target ID
                    if (_cardManager.clearCards[j] != null && instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID 
                        == _cardManager.clearCards[j].GetComponentInChildren<CardDisplay>().ID)
                    {
                        _demoDeck = _deckManagerCard.RemoveAt(_demoDeck, i - numOfClearedCards); //When IDs match, remove the card from the list
                        numOfClearedCards++; //Since a card is removed, list must be indexed to compensate for lost card
                    }
                }
            }
        }

        //If the confirmed card was a switch card
        if (isSwitching)
        {
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
                _demoDeck = _deckManagerCard.Swap(_demoDeck, target1Index, target2Index); //Swaps the two cards
            }
            else
            {
                print("FAILED TO LOCATE CARD IDS");
            }
            _uiManager.UpdatePlayedCards(_demoDeck);

            int demoCount = _demoDeck.Count;
            hasSwitched = false;
            for (int i = 0; i < demoCount; i++)
            {
                if (_demoDeck[i].name != _playedCards[i].name)
                {
                    hasSwitched = true;
                    break;
                }
            }
        }

        PlayDemoActionOrder?.Invoke(_demoDeck);
    }

    /// <summary>
    /// Stops the demo from being played
    /// </summary>
    public void StopDemo()
    {
        PlayDemoActionOrder?.Invoke(new List<Card>());
        _getOriginalDeck = true;
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
        StartCoroutine(DeathTimer());
    }

    /// <summary>
    /// When the player runs out of cards, this method is called
    /// </summary>
    private void OutOfCards()
    {
        ChangeGameState(STATE.Death);
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
                lastCardPlayed = (playedCard, i); //Stores card and the index
                confirmationCard = lastCardPlayed.Item1;
                break;
            }
        }
        RunPlaySequence();
    }

    /// <summary>
    /// If the confirm button is pressed, 
    /// </summary>
    public void ConfirmCards()
    {
        isConfirmCardThere = false;
        _getOriginalDeck = true;
        _shouldMoveDealtCards = true;
        ChangeGameState(STATE.PlayingActionOrder);
        _cardManager.lastConfirmationCard = null;
        _uiManager.cancelButton.GetComponent<ButtonControls>().SetIsActive(false);
        CardManager.Instance.canMoveCard = true;
        SetCursor("Default");


        //Disables Arrows
        isUsingWild = false;
        currentlyOnWild = false;
        isTurning = false;
        _uiManager.UpdateArrows();

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
        if (confirmationCard.name != Card.CardName.Clear && confirmationCard.name != Card.CardName.Switch 
            && confirmationCard.name != Card.CardName.Stall && confirmationCard.name != Card.CardName.Wild)
            _playedCards.Add(confirmationCard);

        _dealtCards.Remove(lastCardPlayed.Item1);

        //If the confirmed card was a clear card
        if (isClearing)
        {
            isClearing = false;

            //sound effect call
            SfxManager.Instance.PlaySFX(6189);

            List<Image> instantiatedImages = _uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

            int demoCount = _demoDeck.Count;
            _playedCards = new();
            for (int i = 0; i < demoCount; i++)
            {
                _playedCards.Add(_demoDeck[i]);
            }

            //Repositions shift index on clear
            int clearCardsLength = _cardManager.clearCards.Length;
            foreach (Image clearCard in _cardManager.clearCards)
            {
                if (clearCard != null)
                    _uiManager.shiftIndex--;
            }

            _cardManager.clearCards = new Image[_cardManager.numOfCardsToClear];
        }

        //If the confirmed card was a switch card
        if (isSwitching)
        {
            isSwitching = false;

            int demoCount = _demoDeck.Count;
            _playedCards = new();
            for (int i = 0; i < demoCount; i++)
            {
                _playedCards.Add(_demoDeck[i]);
            }
        }
        ActionOrderDisplay.shouldReset = true;
        isStalling = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void CancelCard()
    {
        //Stops Demo
        StopDemo();

        //Resets CardManager
        isConfirmCardThere = false;
        _cardManager.lastConfirmationCard = null;
        _cardManager.clearCards = new Image[_cardManager.numOfCardsToClear];
        _cardManager.switchCards.Item1 = null;
        _cardManager.switchCards.Item2 = null;
        CardManager.Instance.canMoveCard = true;
        _shouldMoveDealtCards = false;
        SetCursor("Default");


        //Removes all highlight from the images
        List<Image> tempPlayedCards = _uiManager.GetInstantiatedPlayedCardImages();
        int tempCount = tempPlayedCards.Count;
        for (int i = 0; i < tempCount; i++)
        {
            tempPlayedCards[i].gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
            tempPlayedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
        }

        //Resets to prior state
        if (lowerDarkenIndex)
        {
            darken.transform.SetSiblingIndex(darken.transform.GetSiblingIndex() - 1);
            lowerDarkenIndex = false;
        }
        darken.enabled = false;
        isClearing = false;
        isSwitching = false;
        isStalling = false;
        isUsingWild = false;
        isTurning = false;
        currentlyOnTurn = false;
        currentlyOnWild = false;
        _getOriginalDeck = true;

        _cardManager.RemoveAllHighlight(_uiManager.GetInstantiatedPlayedCardImages()); //Removes the highlight
        _collectedSwitchIDs = new(); //Clears the list
        _uiManager.UpdatePlayedCards(_playedCards);
        _uiManager.UpdateArrows();
        _buttonsManager.ResetIndex();

        _uiManager.cancelButton.GetComponent<ButtonControls>().SetIsActive(false);
        _uiManager.confirmButton.GetComponent<ButtonControls>().SetIsActive(false);

        if (!_gameLost)
        {
            _cardManager.canMoveCard = false;
            StartCoroutine(ReturnAnimation(_uiManager.confirmationImage, new Vector2((_uiManager.cardWidth + 10)
                       * (lastCardPlayed.Item2 + 1) + 15, 15)));
        }
    }

    IEnumerator ReturnAnimation(Image moveImage, Vector2 targetPosition)
    {
        while (moveImage.rectTransform.anchoredPosition != targetPosition)
        {
            moveImage.rectTransform.anchoredPosition = Vector2.MoveTowards(moveImage.rectTransform.anchoredPosition, targetPosition, 30f * Time.deltaTime * 60);
            yield return new WaitForEndOfFrame();
        }

        _uiManager.DestroyConfirmCard();
        ChangeGameState(STATE.ChooseCards);
        _cardManager.canMoveCard = true;
        yield return null;
    }

    /// <summary>
    /// Forces UI played cards display to display _playedCards
    /// This method is to be used if player replaces the Switch or Clear Card with another card
    /// </summary>
    public void ResetPlayedDisplay()
    {
        _uiManager.UpdatePlayedCards(_playedCards);
    }

    /// <summary>
    /// Checks the sequence and calls the animation based off the card
    /// Called after the player chooses a card and when the player returns to the idle animation
    /// </summary>
    public void PlaySequence()
    {
        if (lowerDarkenIndex)
        {
            darken.transform.SetSiblingIndex(darken.transform.GetSiblingIndex() - 1);
            lowerDarkenIndex = false;
        }
        darken.enabled = false;
        //Invokes Action that Eli's script is listening to
        PlayActionOrder?.Invoke(_playedCards);

        if (_doDebugMode && !_gameLost)
            ChangeGameState(STATE.ChooseCards);
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
            _cardManager.switchCards.Item1 = null;
            _cardManager.switchCards.Item2 = null;
        }

    }

    public void WildAction(Card card)
    {
        //Resets state to before card was viewed
        _getOriginalDeck = true;
        _cardManager.clearCards = new Image[_cardManager.numOfCardsToClear];
        _cardManager.switchCards.Item1 = null;
        _cardManager.switchCards.Item2 = null;
        StopDemo();

        //Removes Card Clear and Swap Highlights
        List<Image> tempPlayedCards = _uiManager.GetInstantiatedPlayedCardImages();
        int tempCount = tempPlayedCards.Count;
        for (int i = 0; i < tempCount; i++)
        {
            tempPlayedCards[i].gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
            tempPlayedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
        }

        //Sets variables based on the switched card
        switch (card.name)
        {
            case Card.CardName.Move:
            case Card.CardName.Jump:
            case Card.CardName.TurnRight:
            case Card.CardName.TurnLeft:
                isClearing = false;
                isSwitching = false;
                isStalling = false;
                isUsingWild = true;
                currentlyOnWild = false;
                _getOriginalDeck = true;

                //Deactivates Darken Effect
                if (lowerDarkenIndex)
                {
                    darken.transform.SetSiblingIndex(darken.transform.GetSiblingIndex() - 1);
                    lowerDarkenIndex = false;
                }
                darken.enabled = false;
                break;
            case Card.CardName.Clear:
                isClearing = true;
                isSwitching = false;
                isStalling = false;
                isUsingWild = false;
                currentlyOnWild = true;
                _getOriginalDeck = true;

                //Activates Darken Effect
                if (_playedCards.Count > 0)
                {
                    darken.enabled = true;
                    _uiManager.UpdateTextBox("SELECT A CARD TO CLEAR.");
                }
                else //Disables Darken Effect - Shouldn't ever need this - Failsafe
                    darken.enabled = false;

                //Deactivates Confirm Button
                _uiManager.confirmButton.GetComponent<ButtonControls>().SetIsActive(false);
                break;
            case Card.CardName.Switch:
                isClearing = false;
                isSwitching = true;
                isStalling = false;
                isUsingWild = false;
                currentlyOnWild = true;
                _getOriginalDeck = true;

                //Activates Darken Effect
                if (_playedCards.Count > 1)
                {
                    darken.enabled = true;
                    _uiManager.UpdateTextBox("SELECT TWO CARDS TO SWAP.");
                }
                else //Disables Darken Effect
                    darken.enabled = false;

                //Deactivates Confirm Button
                _uiManager.confirmButton.GetComponent<ButtonControls>().SetIsActive(false);
                break;
            case Card.CardName.Stall:
                isClearing = false;
                isSwitching = false;
                isStalling = true;
                isUsingWild = true;
                currentlyOnWild = false;
                _getOriginalDeck = true;

                //Deactivates Darken Effect
                if (lowerDarkenIndex)
                {
                    darken.transform.SetSiblingIndex(darken.transform.GetSiblingIndex() - 1);
                    lowerDarkenIndex = false;
                }
                darken.enabled = false;
                break;
        }
    }

    public void TurnAction(Card card)
    {
        isClearing = false;
        isSwitching = false;
        isStalling = false;
        isUsingWild = true;
        currentlyOnWild = false;
        currentlyOnTurn = false;
        _getOriginalDeck = true;

        //Deactivates Darken Effect
        if (lowerDarkenIndex)
        {
            darken.transform.SetSiblingIndex(darken.transform.GetSiblingIndex() - 1);
            lowerDarkenIndex = false;
        }
        darken.enabled = false;
    }
    #endregion

    /// <summary>
    /// Starts a new turn
    /// </summary>
    public void NewTurn()
    {
        //Moves Cards if the Card is added into the Action Order (Example: if the card is not a Clear Card)

        if (confirmationCard != null && confirmationCard.name != Card.CardName.Clear && confirmationCard.name != Card.CardName.Switch && confirmationCard.name != Card.CardName.Stall)
        {
            List<Image> playedCardImages = _uiManager.GetInstantiatedPlayedCardImages();
            int playedCardsCount = playedCardImages.Count;
            for (int i = 0; i < playedCardsCount; i++)
            {
                playedCardImages[i].GetComponentInChildren<CardDisplay>().MoveCards(i);
            }
            _uiManager.shiftIndex++;
        }
        if (!_gameLost)
        {
            ChangeGameState(STATE.ChooseCards);
        }
    }

    /// <summary>
    /// Timer to reset the game state
    /// </summary>
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(_deathTimerLength);
        print("DEATH");
        _gameLost = true;
        DeathAction?.Invoke();
        SceneTransitionManager.Instance.ResetLevel();
        yield return null;
    }

    private void LoadLevelSelect()
    {
        LevelSelect ls = FindObjectOfType<LevelSelect>(false);
        ls.LoadLevelOnWin(ls.GetSceneToGoOnWin());
    }
    public void SetCollectableCollected(bool state)
    {
        _collectableCollected = state;
    }
    public bool GetCollectableStatus()
    {
        return _collectableCollected;
    }
    #region Getters
    public int GetCollectableCount() { return collectablesCollected.Count; }

    /// <summary>
    /// Gets the current played cards
    /// </summary>
    /// <returns>A List<Card> Cards that have been played</returns>
    public List<Card> GetStartingCards() { return _startingDeck; }

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
    /// Gets the current deck of the level. 
    /// </summary>
    /// <returns>Returns the cards left in the deck</returns>
    public List<Card> GetDeck() { return deck;  }

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

    public void SetCursor(String name)
    {
        switch (name)
        {
            case "Default":
                Cursor.SetCursor(null, _sunnnyCursorHotspot, CursorMode.Auto);
                currentCursor = "Default";
                break;
            case "Clear":
                Cursor.SetCursor(_clearCursor, _switchCursorHotspot, CursorMode.Auto);
                currentCursor = "Clear";
                break;
            case "Switch":
                Cursor.SetCursor(_switchCursor, _switchCursorHotspot, CursorMode.Auto);
                currentCursor = "Switch";
                break;
            default:
                Debug.LogWarning("Cursor " + name + " not recognized");
                currentCursor= "null";
                break;
        }
    }
    #endregion

    public enum STATE
    {
        LoadGame,
        Menu,
        StartLevel,
        ChooseCards,
        ConfirmCards,
        PlayingActionOrder,
        Trap,
        ChooseTurn,
        Death,
        OutOfCards,
        End
    }
}