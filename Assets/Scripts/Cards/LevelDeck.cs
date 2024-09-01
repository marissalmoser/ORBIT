//
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 1st 2024
// @Description - The controller of the deck for each level
//

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
                instance = FindAnyObjectByType(typeof(LevelDeck )) as LevelDeck;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion


    public List<Card> deck;
    private DeckManager<Card> deckManager;

    public void Init()
    {
        deckManager = DeckManager<Card>.Instance;
        //TODO - Split up deck into two packets. Shuffle the second packet
        if (deck.Count > 4)
        {
            List<Card> tempList = new();

            //Removes all cards from cards after the 4th card
            //TODO - Change int i = 0   to   int i = 4
            for (int i = 0; i < deck.Count; i++)
            {
                tempList.Add(deck[i]);
                deck.RemoveAt(i);
            }

            //Shuffles second packet
            deckManager.Shuffle(tempList);

            //Adds second packet back into the deck
            int tempListSize = tempList.Count;
            for (int i = 0; i < tempListSize; i++)
            {
                deck.Add(tempList[i]);
            }
        }
    }
}