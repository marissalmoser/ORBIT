// +--------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 9 2024
// @Description - Displays the card onto an instantiated image.
//                Also holds helper methods for an Event Trigger
// +--------------------------------------------------------------+

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    [SerializeField] private Image _sprite;
    public int ID;
    public bool IsMouseInCard { get;  private set; }
    public bool IsMouseDown { get; private set; }
    public bool IsClearing { get; private set; }
    public bool IsSwapping { get; private set; }

    private GameManager _gameManager;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _sprite.sprite = card.cardSprite;

        IsMouseInCard = false;
        IsMouseDown = false;
        IsSwapping = false;
    }

    #region Dealt Card Methods

    /// <summary>
    /// Helper method for Event Trigger Pointer Down for DealtCards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseEnterDealtCard(Image tooltip)
    {
        IsMouseInCard = true;
        CardManager.Instance.DealtMouseEnterCard(tooltip);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Down for DealtCards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseExitDealtCard(Image tooltip)
    {
        IsMouseInCard = false;
        CardManager.Instance.DealtMouseExitCard(tooltip);
    }
    /// <summary>
    /// Helper method for Event Trigger Pointer Down for DealtCards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MousePressedDealtCard(Image Card)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            IsMouseDown = true;
            CardManager.Instance.DealtMousePressedCard(Card);
        }
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Up for Dealt Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseReleasedDealtCard(Image Card)
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            IsMouseDown = false;
            CardManager.Instance.DealtMouseReleasedCard(Card, ID);
        }
    }

    /// <summary>
    /// Helper method for Event Trigger Drag for Dealt Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void OnDragDealtCard(Image Card)
    {
        if (Mouse.current.leftButton.isPressed)
        {
            CardManager.Instance.DealtOnDragCard(Card);
        }
    }

    /// <summary>
    /// Helper method for choosing to turn left
    /// </summary>
    public void TurnLeftChosen()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            CardManager.Instance.PlayedTurnChooseLeft();
        }
    }

    /// <summary>
    /// Helper method for choosing to turn right
    /// </summary>
    public void TurnRightChosen()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            CardManager.Instance.PlayedTurnChooseRight();
        }
    }

    /// <summary>
    /// Helper method for mouse entering turn card
    /// </summary>
    public void MouseEnterTurnCard(Image tooltip)
    {
        CardManager.Instance.MouseEnterTurnCard(tooltip);
    }

    /// <summary>
    /// Helper method for mouse leaving turn card
    /// </summary>
    public void MouseExitTurnCard(Image tooltip)
    {
        CardManager.Instance.MouseExitTurnCard(tooltip);
    }
    #endregion

    #region Played Card Methods

    /// <summary>
    /// Helper method for Event Trigger Mouse Down for Played Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MousePressedPlayedCard(Image Card)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            IsMouseDown = true;
            CardManager.Instance.PlayedMousePressedCard(Card);
        }
    }

    /// <summary>
    /// Helper method for Event Trigger Mouse Up for Played Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseReleasedPlayedCard(Image Card)
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            IsMouseDown = false;

            if (_gameManager.isSwitching)
                IsSwapping = !IsSwapping;
            if (_gameManager.isClearing)
                IsClearing = !IsClearing;
        }

        CardManager.Instance.PlayedMouseReleasedCard(Card);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Enter for Played Cards
    /// </summary>
    /// <param name="card">Image object for the card</param>
    public void OnMouseEnterPlayedCard(Image card)
    {
        IsMouseInCard = true;
        CardManager.Instance.PlayedMouseEnterCard(card);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Exit for Played Cards
    /// </summary>
    /// <param name="card">Image object for the card</param>
    public void OnMouseExitPlayedCard(Image card)
    {
        IsMouseInCard = false;
        CardManager.Instance.PlayedMouseExitCard(card);
    }
    #endregion
}