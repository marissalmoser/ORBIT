// +--------------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - October 16th 2024
// @Description - Displays the card onto an instantiated image.
//                Also holds helper methods for an Event Trigger
// +--------------------------------------------------------------+

using System.Collections;
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

    public bool isDarken;
    public bool isFromWild;
    public float doubleClickLength = 0.6f;

    private GameManager _gameManager;

    private Image _cardImage;
    private Animator _anim;
    private bool _isSelected;
    private bool _isDragging;
    public float doubleClickTimer;
    private bool canDoubleClick;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _anim = GetComponentInParent<Animator>();
        _cardImage = GetComponent<Image>();
        IsMouseInCard = false;
        IsMouseDown = false;
        IsSwapping = false;

        doubleClickTimer = 0;
        canDoubleClick = false;

        //Sets image
        SetImage();
    }

    #region Card Getter and Setter
    /**
    /// <summary>
    /// Updates the specified card's image
    /// </summary>
    /// <param name="card">The card to be updared</param>
    public void UpdateCard(Card card, bool isFromWild)
    {
        this.isFromWild = isFromWild;
        //Updates card
        this.card = card;

        SetImage();
    }

    public void UpdateCard(Card card)
    {
        //Updates card
        this.card = card;
        SetImage();
    }
    */

    public void SetImage()
    {
        if (isFromWild)
        {
            if (isDarken)
            {
                _sprite.sprite = card.wildDarkenVariantSprite;
            }
            else //If card is not darkened and from a wild card
            {
                _sprite.sprite = card.wildVariantSprite;
            }
        }
        else //If it is a normal card (not from wild)
        {
            if (isDarken) //If card is darkened and a normal card
            {
                _sprite.sprite = card.darkenVariantSprite;
            }
            else //If card is not darkened and a normal card
            {
                _sprite.sprite = card.cardSprite;
            }
        }
    }

    /// <summary>
    /// Gets the current card
    /// </summary>
    /// <returns>Card object stored inside the card</returns>
    // public Card GetCard() {  return card; }
    #endregion

    #region Deck Methods
    public void MouseEnterShownDeckCard(Image toolTip)
    {
        CardManager.Instance.MouseEnterShownDeckCard(toolTip);
    }

    public void MouseExitShownDeckCard(Image toolTip)
    {
        CardManager.Instance.MouseExitShownDeckCard(toolTip);
    }
    public void MouseReleasedDeck(Image cardImage)
    {
        CardManager.Instance.MouseReleasedDeck(cardImage);
    }
    #endregion

    #region Dealt Card Methods

    /// <summary>
    /// Helper method for Event Trigger Pointer Down for DealtCards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseEnterDealtCard(Image tooltip)
    {
        IsMouseInCard = true;
        CardManager.Instance.MouseEnterDealtCard(tooltip);

        //plays pop up animation if card is playable
        if (_anim != null && CardIsPlayable())
        {
            _anim.SetBool("Hover", true);
        }
        isDarken = true;
        SetImage();
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Down for DealtCards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseExitDealtCard(Image tooltip)
    {
        IsMouseInCard = false;
        CardManager.Instance.MouseExitDealtCard(tooltip);

        //plays pop up animation if card is playable
        if (_anim != null && CardIsPlayable())
        {
            _anim.SetBool("Hover", false);
        }
        isDarken = false;
        SetImage();
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
            CardManager.Instance.MousePressedDealtCard(Card);

            //disable animator to allow drag
            if (_anim != null)
            {
                _anim.enabled = true;
            }
            //double click to play functionality if the card is playable.
            if (CardIsPlayable())
            {
                SelectCard(Card);
            }
            if (!canDoubleClick)
                StartCoroutine(DoubleClick());
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
            CardManager.Instance.MouseReleasedDealtCard(Card, ID);
        }
    }

    /// <summary>
    /// Called when the player clicks on a dealt card that can be played. If it is selected,
    /// playes tha card, it it is not already selected, selects it.
    /// </summary>
    /// <param name="Card"></param>
    private void SelectCard(Image Card)
    {
        if (canDoubleClick)
        {
            //play card and sound
            CardManager.Instance.PlayCard(Card, ID);
            SfxManager.Instance.PlaySFX(4295);

            //TODO: move card to play area

            return;
        }

        //if card is not active yet, deselect any other active dealt cards
        //and make this one active and play anim and sound.
        if(CardIsPlayable())
        {
            foreach (Image dealtCard in UIManager.Instance.GetInstantiatedDealtCardImages())
            {
                dealtCard.GetComponentInChildren<CardDisplay>().SetIsSelected(false);
            }
            _isSelected = true;
            SfxManager.Instance.PlaySFX(1092);
        }
    }

    /// <summary>
    /// Retrns a bool that checks if the card is playable or not. Used to see if the
    /// animations should play or not.
    /// </summary>
    /// <returns></returns>
    private bool CardIsPlayable()
    {
        if (card.name == Card.CardName.Clear && UIManager.Instance.GetInstantiatedPlayedCardImages().Count < 1)
            return false;
        if (card.name == Card.CardName.Switch && UIManager.Instance.GetInstantiatedPlayedCardImages().Count < 2)
            return false;

        return true;
    }

    public IEnumerator DoubleClick()
    {
        canDoubleClick = true;
        doubleClickTimer = doubleClickLength;
        while (doubleClickTimer > 0)
        {
            doubleClickTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        canDoubleClick = false;
        yield return null;
    }

    /// <summary>
    /// Sets the dealt card display and animation for the double click to play functionality.
    /// </summary>
    /// <param name="input"></param>
    public void SetIsSelected(bool input)
    {
        if(_isSelected && !input)
        {
            _anim.SetBool("Select", false);
        }
        if(!_isSelected && input)
        {
            _anim.SetBool("Select", true);
        }
        _isSelected = input;
    }

    /// <summary>
    /// Helper method for Event Trigger Drag for Dealt Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void OnDragDealtCard(Image Card)
    {
        if (Mouse.current.leftButton.isPressed && CardIsPlayable())
        {
            CardManager.Instance.OnDragDealtCard(Card);

            //disable animator to allow drag
            if (_anim != null)
            {
                _anim.enabled = false;
            }
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
            CardManager.Instance.MousePressedPlayedCard(Card);
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

        CardManager.Instance.MouseReleasedPlayedCard(Card);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Enter for Played Cards
    /// </summary>
    /// <param name="card">Image object for the card</param>
    public void OnMouseEnterPlayedCard(Image card)
    {
        IsMouseInCard = true;
        CardManager.Instance.MouseEnterPlayedCard(card);
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Exit for Played Cards
    /// </summary>
    /// <param name="card">Image object for the card</param>
    public void OnMouseExitPlayedCard(Image card)
    {
        IsMouseInCard = false;
        CardManager.Instance.MouseExitPlayedCard(card);
    }
    #endregion

    /// <summary>
    /// Does the animation to move the cards to the left when confirming a card
    /// </summary>
    public void MoveCards(int delay)
    {
            StartCoroutine(MoveAnimation(delay));
    }

    IEnumerator MoveAnimation(int delay)
    {
        yield return new WaitForSeconds(delay * 0.1f);
        Vector2 targetPosition = _cardImage.rectTransform.anchoredPosition - new Vector2(120, 0);

        while (_cardImage.rectTransform.anchoredPosition.x != targetPosition.x)
        {
            _cardImage.rectTransform.anchoredPosition = Vector2.MoveTowards(_cardImage.rectTransform.anchoredPosition, targetPosition, 7f);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}