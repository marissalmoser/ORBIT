// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 4 2024
// @Description - The controller of the deck for each level
// +-------------------------------------------------------+

using System.Collections.Generic;
using UnityEngine;

public class LevelDeck : MonoBehaviour
{
    #region Singleton
    private static LevelDeck instance;
    public static LevelDeck Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(LevelDeck)) as LevelDeck;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion


    public List<Card> deck;
    [SerializeField] private List<Card> originalDeck;
    private DeckManager<Card> _deckManager;

    /// <summary>
    /// Initializes variables for LevelDeck. Called by GameManager
    /// </summary>
    public void Init()
    {
        _deckManager = DeckManager<Card>.Instance;

        int deckCount = deck.Count;

        //TODO - Split up deck into two packets. Shuffle the second packet
        if (deck.Count > 4)
        {
            List<Card> tempList = new();

            //Removes all cards from cards after the 4th card
            //TODO - Change int i = 0   to   int i = 4
            for (int i = 4; i < deckCount; i++)
            {
                tempList.Add(deck[4]);
                deck.RemoveAt(4);
            }

            //Shuffles second packet
            _deckManager.Shuffle(tempList);

            //Adds second packet back into the deck
            int tempListSize = tempList.Count;
            for (int i = 0; i < tempListSize; i++)
            {
                deck.Add(tempList[i]);
            }
        }
        for (int i = 0; i < deckCount; i++)
        {
            originalDeck.Add(deck[i]);
        }

        //Use deckManager.PrintDeck(deck); here to see the shuffled deck
    }

    public void ResetDeck()
    {
        int originalDeckCount = originalDeck.Count;
        deck.Clear();
        for (int i = 0; i < originalDeckCount; i++)
        {
            deck.Add(originalDeck[i]);
        }
    }
}