// +-----------------------------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last Modified - September 1st 2024
// @Description - The engine of the game which controls and initializes everything else
// +-----------------------------------------------------------------------------------+

using System.Collections.Generic;
using UnityEngine;

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

    DeckManager<Card> deckManager;
    UIManager uiManager;

    public STATE gameState;

    private LevelDeck levelDeck;
    private List<Card> deck;
    private List<Card> dealtCards;
    private List<Card> playedCards;
    private List<Card> tempPlayedCards;
    private List<Card> tempBeforeBackToItCards, tempAfterBackToItCards;
    private int lastBackToItIndex;

    private void Start()
    {
        deckManager = DeckManager<Card>.Instance;
        uiManager = UIManager.Instance;
        lastBackToItIndex = -1;
    }

    /// <summary>
    /// Controls the current game state.
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
                gameState = STATE.ChooseClear;
                break;
            case STATE.SwitchCards:
                gameState = STATE.SwitchCards;
                break;
            case STATE.End:
                gameState = STATE.End;
                //Add method here if needed
                break;
            default:
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
        uiManager.Init();
        levelDeck.Init();

        deck = new();
        dealtCards = new();
        playedCards = new();
        tempPlayedCards = new();
        tempBeforeBackToItCards = new();
        tempAfterBackToItCards = new();
    }

    /// <summary>
    /// Sets up the current level
    /// Called when level is selected
    /// </summary>
    private void SetUpLevel()
    {
        deck = levelDeck.deck;
    }

    /// <summary>
    /// Gets how many dealt cards there are and draws up to max hand size (4)
    /// Called at the beginning of every turn
    /// </summary>
    private void DealCards()
    {
        int dealtCardsCount = dealtCards.Count;
        int deckSize = deck.Count;

        //Draws until the player has 4 cards or until the deck runs out
        while (dealtCardsCount < 4 && deckSize > 0)
        {
            //TODO - Keep Cards in same place, replace played card's index with new card
            dealtCards.Add(deck[0]);
            deck = deckManager.DealCard(deck);
        }
    }

    /// <summary>
    /// Runs the current action order
    /// Called after a card has been played
    /// </summary>
    private void RunPlaySequence()
    {
        lastBackToItIndex = -1;

        tempPlayedCards.Clear();

        int playedCardsCount = playedCards.Count;

        //Adds copies of the cards into a temporary list
        for (int i = 0; i < playedCardsCount; i++)
        {
            tempPlayedCards.Add(playedCards[i]);
        }

        //If Clear Card was Played
        if (playedCards.Count > 0 && playedCards[playedCards.Count - 1].name == Card.CardName.Clear)
        {
            //TODO
        }

        //If Switch Card was played
        if (playedCards.Count > 0 && playedCards[playedCards.Count - 1].name == Card.CardName.Switch)
        {
            //TODO
        }
    }

    /// <summary>
    /// Checks the sequence and calls the animation based off the card
    /// Called after the player chooses a card and when the player returns to the idle animation
    /// </summary>
    public void PlaySequence()
    {
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
    }

    /// <summary>
    /// Functionality of the Clear Card
    /// </summary>
    private void ClearAction()
    {

    }

    /// <summary>
    /// Functionality of the Switch Card
    /// </summary>
    private void SwitchAction()
    {

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

    public enum STATE
    {
        LoadGame,
        Menu,
        StartLevel,
        ChooseCards,
        RunActionOrder,
        ChooseClear,
        SwitchCards,
        End
    }
}