using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Image sprite;
    public int ID;

    GameManager gameManager;
    void Start()
    {
        gameManager = GameManager.Instance;
        sprite.sprite = card.cardSprite;
        //TODO - Check which deck the card is in (playedCards or dealtCards). Give ID, set other ID to -1
        //Sets the Card ID
    }

    public void UpdateCard(Card card)
    {
        this.card = card;

        gameManager = GameManager.Instance;
        ID = card.ID;
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Down
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MousePressedDealtCard(Image Card)
    {
        DealtCardManager.Instance.MousePressedCard(Card);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Up
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseReleasedDealtCard(Image Card)
    {
        DealtCardManager.Instance.MouseReleasedCard(Card, ID);
    }

    /// <summary>
    /// Helper method for Event Trigger Drag
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void OnDragDealtCard(Image Card)
    {
        DealtCardManager.Instance.OnDragCard(Card);
    }

    public void MousePressedPlayedCard(Image card)
    {
        PlayedCardManager.Instance.MousePressedCard(card);
    }
}