// +-----------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last Modifieed - August 31 2024
// @Description - A script with universal functions for a Generic deck
// +-----------------------------------------------------------------+
using System.Collections.Generic;
using UnityEngine;

//Generic Class
public class DeckManager<T> : MonoBehaviour
{
    //Making Singleton Class
    #region Singleton
    private static DeckManager<T> instance;
    public static DeckManager<T> Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(DeckManager<T>)) as DeckManager<T>;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    #region Shuffle
    /**
     * Shuffles the deck
     * @param List<T> deck - the deck to be shuffled
     * @return List<T> - the shuffled deck
     */
    public List<T> Shuffle(List<T> deck)
    {
        //Checks if the deck is valid
        if (deck == null || deck.Count < 1)
            return deck;

        int deckSize = deck.Count; //Size of the deck
        int numTimesToShuffleDeck = Random.Range(10, 16);

        //Randomly shuffles the deck a number of times
        for (int i = 0; i < numTimesToShuffleDeck; i++)
        {
            //Performs a riffle shuffle a random amount of times (between 7 and 15 times)
            int numTimesToDoShuffle = Random.Range(7, 16);
            for (int j = 0; j < numTimesToDoShuffle; j++)
            {
                deck = RiffleShuffle(deck);
            }
            //Performs an over hand shuffle a random amount of times (between 7 and 15 times)
            numTimesToDoShuffle = Random.Range(7, 16);
            for (int j = 0; j < numTimesToDoShuffle; j++)
            {
                deck = OverHandShuffle(deck);
            }

            //Cuts the deck
            CutDeck(deck);
        }

        return deck;
    }

    #region Riffle Shuffle
    /**
     * Performs a riffle shuffle onto a deck
     * @param deck - The deck to perform a riffle shuffle on
     * @return List<T> - the shuffled deck
     */
    public List<T> RiffleShuffle(List<T> deck)
    {
        //Checks if the deck is valid
        if (deck == null || deck.Count < 1)
            return deck;

        int deckSize = deck.Count; //Size of the deck

        List<T> topHalf = new();
        List<T> bottomHalf = new();
        T tempObject = default(T);

        //Checks for an uneven number of objects
        if (deckSize % 2 == 1)
            tempObject = deck[deckSize - 1];

        //Splits the deck into two different packets
        for (int i = 0; i < deckSize / 2; i++)
        {
            topHalf.Add(deck[i]);
        }
        for (int i = deckSize/ 2; i < deckSize; i++)
        {
            bottomHalf.Add(deck[i]);
        }

        //Clears deck
        deck.Clear();

        //Adds objects back into the deck

        //Adds in the tempObject if it isn't null
        if (tempObject != null)
            deck.Add(tempObject);

        //Adds the packets back into the deck
        //Interlaces two packets into each other starting from the bottom
        for (int i = deckSize / 2 - 1; i > -1; i--)
        {
            deck.Add(bottomHalf[i]);
            deck.Add(topHalf[i]);
        }

        return deck;
    }
    #endregion

    #region Overhand Shuffle
    /**
     * Performs checks on the deck and decides if the overhand shuffle is needed
     * @param deck - The deck to perform an over hand shuffle on
     * @return List<T> - the shuffled deck
     */
    public List<T> OverHandShuffle(List<T> deck)
    {
        //Checks if the deck is valid
        if (deck == null || deck.Count < 1)
            return deck;

        int deckSize = deck.Count;

        //Changes action based on deck size
        switch(deckSize)
        {
            case 1:
                break;
            case 2:
                int randomTimes = Random.Range(1, 5);
                for(int i = 0; i < randomTimes; i++)
                {
                    //Swaps position of objects
                    (deck[1], deck[0]) = (deck[0], deck[1]);
                }
                break;
            case 3:
                //Performs micro overhand shuffle
                int startIndex = Random.Range(1, 3);
                List<T> tempDeck = new();

                //Splits deck into two packets
                for (int i = startIndex; i < deckSize; i++)
                {
                    tempDeck.Add(deck[startIndex]); //Adds objects into second packet
                    deck.RemoveAt(startIndex); //Removes objects from original packet
                }
                int inTempDeck = tempDeck.Count;

                //Reverses the object order to add back into the deck
                for (int i = inTempDeck - 1; i >= 0; i--)
                {
                    deck.Add(tempDeck[i]);
                    tempDeck.RemoveAt(i);
                }
                break;
            case >= 4:
                doOverHandShuffle(deck);
                break;
        }

        return deck;
    }

    /**
     * Performs the over hand shuffle
     * @param List<T> deck - the deck to perform the over hand shuffle on
     * @return List<T> - the shuffled deck
     */
    private List<T> doOverHandShuffle(List<T> deck)
    {
        //Checks if deck is valid
        if (deck == null || deck.Count < 1)
            return deck;

        int deckSize = deck.Count;
        List<T> tempDeck = new();

        //Init variables
        int startIndex;
        bool smallDeck; //A deck smaller than or equal to 15 cards

        //Changes start index random parameters if deck is small
        switch (deckSize)
        {
            case 4:
                startIndex = 1;
                smallDeck = true;
                break;
            case < 7:
                startIndex = Random.Range(1, 3);
                smallDeck = true;
                break;
            case < 11:
                startIndex = Random.Range(1, 4);
                smallDeck = true;
                break;
            case < 16:
                startIndex = Random.Range(1, 5);
                smallDeck = true;
                break;
            case >= 16:
                startIndex = Random.Range(1, 6);
                smallDeck = false;
                break;
        }

        //Splits deck into two separate packets
        for (int i = startIndex; i < deckSize; i++)
        {
            tempDeck.Add(deck[startIndex]); //Adds objects into second packet
            deck.RemoveAt(startIndex); //Removes objects from original packet
        }

        //Sets starting variables
        int inTempDeck = tempDeck.Count;
        int cardsToAddBackIn;

        //Checks if the deck is small
        //Repeats process until all objects in the temp deck are put back into the original deck
        while (inTempDeck > 0)
        {
            //Changes random parameters if the deck is small
            if (smallDeck)
            {
                //If there is 2 or less objects left to be put into the deck, add them all in
                if (inTempDeck < 3)
                    cardsToAddBackIn = inTempDeck;
                else
                    cardsToAddBackIn = Random.Range(1, 3);
            }
            else
            {
                //If there is 4 or less objects left to be put into the deck, add them all in
                if (inTempDeck < 5)
                    cardsToAddBackIn = inTempDeck;
                else
                    cardsToAddBackIn = Random.Range(1, 6);
            }

            //Reverses the object order to add back into the deck
            for (int i = cardsToAddBackIn - 1; i >= 0; i--)
            {
                deck.Add(tempDeck[i]);
                tempDeck.RemoveAt(i);
            }

            //Decrements inTempDeck by the amount taken out
            inTempDeck -= cardsToAddBackIn;
        }

        return deck;
    }
    #endregion

    #region Cut Shuffle
    /**
     * Performs a cut on the deck at a random location
     * @param deck - The deck to perform a cut on
     * @return List<T> - the cutted deck
     */
    public List<T> CutDeck(List<T> deck)
    {
        //Checks if deck is valid
        if (deck == null || deck.Count < 1)
            return deck;

        int deckSize = deck.Count;
        switch(deckSize)
        {
            case 1:
                ///Does nothing
                break;
            case 2:
                //Swaps position of objects
                (deck[1], deck[0]) = (deck[0], deck[1]);
                break;
            case >= 3:
                deck = CutDeck(deck, Random.Range(1, deckSize - 1));
                break;
        }

        return deck;
    }


    /**
     * Performs a cut onto a deck at the specified location
     * @param deck - The deck to perform a cut on
     * @param cutIndex - the index to cut the deck at
     * @return List<T> - the cutted deck
     */
    public List<T> CutDeck(List<T> deck, int cutIndex)
    {
        //Does nothing if cut index is out of bounds
        if (cutIndex < 0 && cutIndex < deck.Count)
        {
            print("CUT INDEX OUT OF BOUNDS");
            return deck;
        }

        List<T> tempDeck = new();

        for(int i = 0; i < cutIndex; i++)
        {
            //Adds object to the temporary deck
            tempDeck.Add(deck[0]);

            //Removes object from the original deck
            deck.RemoveAt(0);
        }

        //Places top half of the deck at the bottom
        for (int i = 0; i < cutIndex; i++)
        {
            deck.Add(tempDeck[i]);
        }

        return deck;
    }
    #endregion
    #endregion

    #region Utility

    /**
     * Deals a card from the top of the deck by removing the top card of the deck
     * @param List<T> deck - the deck to be dealt from
     * @return List<T> - the deck with the dealt card
     */
    public List<T> DealCard(List<T> deck)
    {
        //Checks if the deck is null or has nothing in it
        if (deck == null || deck.Count < 1)
            print("CANNOT DEAL CARD!");
        else
            deck.RemoveAt(0);
        return deck;
    }

    /**
     * Swaps two objects in the deck
     * @param target1Index - The index of the first object to be swapped
     * @param target2Index - the index of the second object to be swapped
     * @return List<T> - The deck with the two objects swapped
     */
    public List<T> Swap(List<T> deck, int target1Index, int target2Index)
    {
        if (deck == null || deck.Count < 1)
            return deck;

        int deckSize = deck.Count;

        //Checks if indecies are in range and are not the same index
        if (target1Index > -1 && target1Index <  deckSize
            && target2Index > -1 && target2Index < deckSize
            && target1Index != target2Index)
        {
            //Swaps the cards
            (deck[target2Index], deck[target1Index]) = (deck[target1Index], deck[target2Index]);
        }

        return deck;
    }

    /**
     * Clears the deck
     * @param deck - the deck to be cleared
     * @return List<T> - an empty generic list
     */
    public List<T> ClearDeck(List<T> deck)
    {
        if (deck == null || deck.Count < 1)
            return deck;

        deck.Clear();
        return deck;
    }

    /**
     * Prints the deck
     * A ToString() method is highly encouraged
     * @param deck - the deck to be printed into the console
     */
    public void PrintDeck(List<T> deck)
    {
        if (deck == null || deck.Count < 1)
            return;

        int deckSize = deck.Count;
        for (int i = 0; i < deckSize; i++)
        {
            print(deck[i]);
        }
    }

    #endregion
}