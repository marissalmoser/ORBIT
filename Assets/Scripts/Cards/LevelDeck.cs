using System.Collections.Generic;
using UnityEngine;

public class LevelDeck : MonoBehaviour
{
    [SerializeField] List<Card> cards;

    private void Start()
    {
        DeckManager<Card> deckManager = new DeckManager<Card>();

        deckManager.Shuffle(cards);

        deckManager.PrintDeck(cards);
    }
    
}