// +-----------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last Modified - September 4st 2024
// @Description - The engine of the game which controls and initializes everything else
// +-----------------------------------------------------------------------------------+

using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private List<Card> dealtCards;
    [SerializeField] private List<Card> playedCards;

    DeckManager<Card> deckManagerCard;
    DeckManager<int> deckManagerInt;
    DealtCardManager dealtCardManager;
    PlayedCardManager playedCardManager;
    UIManager uiManager;

    public STATE gameState;

    private LevelDeck levelDeck;
    private List<Card> deck;

    private List<Card> tempPlayedCards;
    private List<Card> tempBeforeBackToItCards, tempAfterBackToItCards;
    private List<int> collectedSwitchIDs;
    private (Card, int) lastCardPlayed;
    private int lastBackToItIndex;

    private void Start()
    {
        //Carefully change order if needed. Some managers must be initialzed before others
        deckManagerCard = DeckManager<Card>.Instance;
        deckManagerInt = DeckManager<int>.Instance;
        dealtCardManager = DealtCardManager.Instance;
        playedCardManager = PlayedCardManager.Instance;
        uiManager = UIManager.Instance;
        levelDeck = FindObjectOfType<LevelDeck>();
        lastBackToItIndex = -1;

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
                gameState = STATE.LoadGame;
                StartGame();
                break;
            case STATE.Menu:
                //Menu methods called
                gameState = STATE.Menu;
                break;
            case STATE.StartLevel:
                gameState =STATE.StartLevel;
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
            case STATE.Failure:
                gameState = STATE.Failure;
                Failure();
                break;
            case STATE.OutOfCards:
                gameState = STATE.OutOfCards;
                OutOfCards();
                break;
            case STATE.End:
                gameState = STATE.End;
                //Add method here if needed
                break;
            default:
                //Error check
                print("ERROR: FAILED TO SWITCH GAME STATE.");
                break;
        }
    }

    /// <summary>
    /// Initializes everything needed for the game
    /// Called at the start of the game
    /// </summary>
    private void StartGame()
    {
        //Initialzes other managers. Order may matter
        dealtCardManager.Init();
        playedCardManager.Init();
        uiManager.Init();
        levelDeck.Init();

        //Initializes lists.
        deck = new();
        dealtCards = new();
        playedCards = new();
        tempPlayedCards = new();
        tempBeforeBackToItCards = new();
        tempAfterBackToItCards = new();

        collectedSwitchIDs = new();

        ChangeGameState(STATE.StartLevel);
    }

    /// <summary>
    /// Sets up the current level
    /// Called when level is selected
    /// </summary>
    private void SetUpLevel()
    {
        deck = levelDeck.deck;

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
        while (dealtCards.Count < 4 && deck.Count > 0)
        {
            if (lastCardPlayed.Item1 != null)
            {
                //Replaces the last played card with a new card in the same index
                //Keeps the other dealt cards in the same location
                dealtCards.Insert(lastCardPlayed.Item2, deck[0]);
            }
            else
            {
                //Adds the top card from the deck onto the dealtCards
                dealtCards.Add(deck[0]);
            }
            deck = deckManagerCard.RemoveFirst(deck); //Removes the now dealt card from the deck
        }

        //If out of cards, go to corresponding game state
        if (dealtCards.Count == 0 && deck.Count == 0)
        {
            ChangeGameState(STATE.OutOfCards);
        }
        uiManager.UpdateDealtCards(); //Updates Cards
    }

    /// <summary>
    /// Removes a card from the dealt deck, and adds it to the played deck
    /// </summary>
    /// <param name="targetID">The ID of the card to play</param>
    public void PlayCard(int targetID)
    {
        //Due to the Cards being Scriptable Objects, I couldn't find a good way to give them unique IDs
        //So I gave the images unique IDs and used those. Weird way, but it works

        List<Image> instantiatedImages = uiManager.GetInstantiatedDealtCardImages(); //Gets instantiated dealt card images

        int instantiatedImagesCount = instantiatedImages.Count;
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == targetID) //Compares instantiated images' unique ID to the target ID
            {
                Card playedCard = dealtCards[i]; //Gets the same ID card
                lastCardPlayed = (playedCard, i); //Stores card and the index
                dealtCards = deckManagerCard.RemoveAt(dealtCards, i); //Removes card from the dealt deck

                playedCards.Add(playedCard); //Adds card to the played deck

                break;
            }
        }

        uiManager.UpdatePlayedCards();
        ChangeGameState(STATE.RunActionOrder);
    }

    /// <summary>
    /// Runs the current action order
    /// Called after a card has been played
    /// </summary>
    private void RunPlaySequence()
    {
        lastBackToItIndex = -1;

        tempPlayedCards.Clear(); //Clears temporary action order

        int playedCardsCount = playedCards.Count;

        //Adds copies of the cards into a temporary list
        for (int i = 0; i < playedCardsCount; i++)
        {
            tempPlayedCards.Add(playedCards[i]);
        }

        //If Clear Card was Played
        if (playedCards.Count > 0 && playedCards[playedCards.Count - 1].name == Card.CardName.Clear) //Error check and checks if last card played was a Clear
        {
            playedCards = deckManagerCard.RemoveLast(playedCards); //Removes clear card from played carsds
            uiManager.UpdatePlayedCards(); //Updates played cards so clear card does not appear
            if (playedCards.Count > 0) //If there is a card to clear, call ClearAction
            {
                ChangeGameState(STATE.ChooseClear); //Waits for User Input to Clear a Card
            }
        }

        //If Switch Card was played
        if (playedCards.Count > 0 && playedCards[playedCards.Count - 1].name == Card.CardName.Switch) //Error check and checks if last card played was a Switch
        {
            playedCards = deckManagerCard.RemoveLast(playedCards); //Removes switch card from played cards
            uiManager.UpdatePlayedCards(); //Updates played cards so switch card does not appear
            if (playedCards.Count > 1) //Performs switch if there are at least two cards in the deck
            {
                ChangeGameState(STATE.SwitchCards); //Waits for User Input to Switch two cards
            }
        }

        if (gameState == STATE.RunActionOrder)
            PlaySequence(); //Plays the action order
    }

    /// <summary>
    /// Checks the sequence and calls the animation based off the card
    /// Called after the player chooses a card and when the player returns to the idle animation
    /// </summary>
    public void PlaySequence()
    {
        /**
        //If there is nothing in the played deck, play sequence is over and allow the player to pick another card
        if (tempPlayedCards.Count < 1)
        {
            ChangeGameState(STATE.ChooseCards);
            return;
        }

        //Checks the first card in the played cards List and gets its data
        switch (tempPlayedCards[0].name)
        {
            case Card.CardName.Move:
                //TODO - MOVE
                //print("MOVED");
                break;
            case Card.CardName.Jump:
                //TODO - JUMP
                //print("JUMPED");
                break;
            case Card.CardName.Turn:
                //print("TURNED RIGHT");
                //TODO - TURN
                break;
            case Card.CardName.BackToIt:
                //print("BACKED TO IT);
                BackToItAction();
                break;
            default:
                print("ERROR: ATTEMPTED TO DO INVALID ACTION FROM INVALID CARD NAME");
                break;
        }
        //Removes the first card in the action order.
        tempPlayedCards.RemoveAt(0);
        **/

        //TODO - Get Eli's State Machine and send in action order

        ChangeGameState(STATE.ChooseCards);
    }

    /// <summary>
    /// Functionality of the Clear Card
    /// </summary>
    public void ClearAction(int targetID)
    {
        //Due to the Cards being Scriptable Objects, I couldn't find a good way to give them unique IDs
        //So I gave the images unique IDs and used those. Weird way, but it works

        List<Image> instantiatedImages = uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

        int instantiatedImagesCount = instantiatedImages.Count;
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == targetID) //Compares instantiated images' unique ID to the target ID
            {
                playedCards = deckManagerCard.RemoveAt(playedCards, i); //When IDs match, remove the card from the list
                break;
            }
        }
        
        uiManager.UpdatePlayedCards();
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

        List<Image> instantiatedImages = uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

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
            playedCards = deckManagerCard.Swap(playedCards, target1Index, target2Index); //Swaps the two cards
        }
        
        uiManager.UpdatePlayedCards();

        ChangeGameState(STATE.RunActionOrder);
    }

    //Switch List Variable Initialization


    /// <summary>
    /// Receives user input and does checks until conditions are met to continue
    /// </summary>
    /// <param name="targetID">The ID of one of the cards to switch</param>
    public void SwitchActionHelper(int targetID)
    {
        List<Image> instantiatedImages = uiManager.GetInstantiatedPlayedCardImages(); //Gets the instantiated played cards images

        int instantiatedImagesCount = instantiatedImages.Count;
        for (int i = 0; i < instantiatedImagesCount; i++)
        {
            if (instantiatedImages[i].GetComponentInChildren<CardDisplay>().ID == targetID) //Compares instantiated images' unique ID to the target ID
            {
                int collectedIdsListCount = collectedSwitchIDs.Count;

                //If the list contains the target ID already
                if (collectedSwitchIDs.Contains(targetID))
                {
                    //There can only be two Cards in the list. If there is a duplicate ID, it must be the first card
                    collectedSwitchIDs = deckManagerInt.RemoveFirst(collectedSwitchIDs); 
                }
                //If the list does not contain the target ID
                else
                {
                    collectedSwitchIDs.Add(targetID);
                }
                
                break;
            }
        }

        //If two cards have been selected, perform switch action
        if (collectedSwitchIDs.Count == 2)
            SwitchAction(collectedSwitchIDs[0], collectedSwitchIDs[1]);
    }

    /// <summary>
    /// When the player fails, this method is called
    /// </summary>
    private void Failure()
    {
        print("Rip Bozo."); //TODO - Replace this with actual functionality
    }

    /// <summary>
    /// When the player runs out of cards, this method is called
    /// </summary>
    private void OutOfCards()
    {
        print("Skill Issue."); //TODO - Replace this with actual functionality
    }

    /// <summary>
    /// Functionality of the Back To It Card
    /// </summary>
    private void BackToItAction()
    {
        tempBeforeBackToItCards.Clear();
        tempAfterBackToItCards.Clear();
        //Gets index of BackToIt card
        int playedCardsSize = playedCards.Count;

        //If this is first instance of BackToIt in playedCards
        if (lastBackToItIndex == -1)
        {
            //Gets the first instance of BackToIt's index in playedCards
            for (int i = 0; i < playedCardsSize; i++)
            {
                if (playedCards[i].name == Card.CardName.BackToIt)
                {
                    lastBackToItIndex = i;
                    break;
                }
            }
        }
        //If this is the second or greater instance of BackToIt in playedCards
        else
        {
            //Gets the next instance of BackToIt's index in playedCards
            for (int i = lastBackToItIndex + 1; i < playedCardsSize; i++)
            {
                if (playedCards[i].name == Card.CardName.BackToIt)
                {
                    lastBackToItIndex = i;
                    break;
                }
            }
        }

        //Copies all cards before BackToIt Card
        for (int i = 0; i < lastBackToItIndex; i++)
        {
            //Ignores previous Back To It Cards
            if (playedCards[i].name != Card.CardName.BackToIt)
            {
                tempBeforeBackToItCards.Add(playedCards[i]);
            }
        }
        //Copies all cards after BackToIt Card
        for (int i = lastBackToItIndex + 1; i < playedCardsSize; i++)
        {
            tempAfterBackToItCards.Add(playedCards[i]);
        }

        //Clears the tempPlayedCardsList
        tempPlayedCards.Clear();

        //Adds card to be removed at front of list ( due to RemoveAt(0) at the end of PlaySequence() )
        tempPlayedCards.Add(playedCards[lastBackToItIndex]);

        //Adds all cards before Back To It into the list
        int beforeBackToItSize = tempBeforeBackToItCards.Count;
        for (int i = 0; i < beforeBackToItSize; i++)
        {
            tempPlayedCards.Add(tempBeforeBackToItCards[i]);
        }

        //Adds all cards after Back To It into the list
        int afterBackToItSize = tempAfterBackToItCards.Count;
        for (int i = 0; i < afterBackToItSize; i++)
        {
            tempPlayedCards.Add(tempAfterBackToItCards[i]);
        }
    }

    /// <summary>
    /// Gets the current dealt cards
    /// </summary>
    /// <returns>A List<Card> Cards that have been dealt</returns>
    public List<Card> GetDealtCards() { return dealtCards; }


    /// <summary>
    /// Gets the current played cards
    /// </summary>
    /// <returns>A List<Card> Cards that have been played</returns>
    public List<Card> GetPlayedCards() { return playedCards; }

    /// <summary>
    /// Gets the current collected switch IDs
    /// </summary>
    /// <returns>List<int> list of IDs from played cards being selected to be swapped</returns>
    public List<int> GetCollectedSwitchIDs() { return collectedSwitchIDs; }

    public enum STATE
    {
        LoadGame,
        Menu,
        StartLevel,
        ChooseCards,
        RunActionOrder,
        ChooseClear,
        SwitchCards,
        Failure,
        OutOfCards,
        End
    }
}