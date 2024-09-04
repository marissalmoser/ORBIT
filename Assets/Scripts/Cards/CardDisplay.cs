// +--------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 4 2024
// @Description - Displays the card onto an instantiated image.
//                Also holds helper methods for an Event Trigger
// +--------------------------------------------------------------+

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
        
    }

    /// <summary>
    /// Updates the specified card's image
    /// </summary>
    /// <param name="card">The card to be updared</param>
    public void UpdateCard(Card card)
    {
        this.card = card;

        gameManager = GameManager.Instance;
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Down for DealtCards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MousePressedDealtCard(Image Card)
    {
        DealtCardManager.Instance.MousePressedCard(Card);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Up for Dealt Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseReleasedDealtCard(Image Card)
    {
        DealtCardManager.Instance.MouseReleasedCard(Card, ID);
    }

    /// <summary>
    /// Helper method for Event Trigger Drag for Dealt Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void OnDragDealtCard(Image Card)
    {
        DealtCardManager.Instance.OnDragCard(Card);
    }

    /// <summary>
    /// Helper method for Event Trigger Mouse Down for Played Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MousePressedPlayedCard(Image Card)
    {
        PlayedCardManager.Instance.MousePressedCard(Card, ID);
    }

    /// <summary>
    /// Helper method for Event Trigger Mouse Up for Played Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseReleasedPlayedCard(Image Card)
    {
        PlayedCardManager.Instance.MouseReleasedCard(Card);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Enter for Played Cards
    /// </summary>
    /// <param name="card">Image object for the card</param>
    public void OnMouseEnterPlayedCard(Image card)
    {
        PlayedCardManager.Instance.MouseEnterCard(card);
    }
}