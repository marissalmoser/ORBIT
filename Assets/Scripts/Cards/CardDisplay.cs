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
    public bool isSelected;
    private bool _isDragging;
    public float doubleClickTimer;
    public bool canDoubleClick;

    private bool isHovered;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _cardImage = GetComponent<Image>();
        IsMouseInCard = false;
        IsMouseDown = false;
        IsSwapping = false;
        isHovered = false;

        doubleClickTimer = 0;
        canDoubleClick = false;

        //Sets image
        SetImage();
    }

    #region Card Getter and Setter
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
        if (CardIsPlayable())
        {
            Hover(true);
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
        if (CardIsPlayable())
        {
            Hover(false);
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
        if (CardManager.Instance.canMoveCard && !IsMouseDown)
        {
            if (_gameManager.gameState == GameManager.STATE.ChooseCards
                || _gameManager.gameState == GameManager.STATE.ConfirmCards
                || _gameManager.gameState == GameManager.STATE.ChooseTurn)
            {
                    IsMouseDown = true;
                    CardManager.Instance.MousePressedDealtCard(Card);
            }
        }
    }

    /// <summary>
    /// Helper method for Event Trigger Pointer Up for Dealt Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void MouseReleasedDealtCard(Image Card)
    {
        if (IsMouseDown && CardManager.Instance.canMoveCard)
        {
            IsMouseDown = false;
            CardManager.Instance.MouseReleasedDealtCard(Card, ID);

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
    /// Called when the player clicks on a dealt card that can be played. If it was 
    /// double clicked on, it plays it
    /// </summary>
    /// <param name="Card"></param>
    private void SelectCard(Image Card)
    {
        if (canDoubleClick)
        {
            //play card and sound
            CardManager.Instance.PlayCard(Card, ID);
            SfxManager.Instance.PlaySFX(4295);
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
    /// Helper method for Event Trigger Drag for Dealt Cards
    /// </summary>
    /// <param name="Card">Image object for the card</param>
    public void OnDragDealtCard(Image Card)
    {
        if (CardIsPlayable() && CardManager.Instance.canMoveCard && IsMouseDown)
        {
            CardManager.Instance.OnDragDealtCard(Card);

            //disable animator to allow drag
            Hover(false);
        }
    }

    public void Hover(bool isHovering)
    {
        if (!isHovered && isHovering)
        {
            StartCoroutine(StartHover());
            isHovered = true;
        }
        else if (isHovered && !isHovering)
        {
            StartCoroutine(CancelHover());
            isHovered = false;
        }
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
        GetComponent<Animator>().SetTrigger("tilt");

        while (_cardImage.rectTransform.anchoredPosition.x != targetPosition.x)
        {
            _cardImage.rectTransform.anchoredPosition = Vector2.MoveTowards(_cardImage.rectTransform.anchoredPosition, targetPosition, 7f);
            RectTransform tooltip = _cardImage.gameObject.transform.parent.Find("Tooltip").GetComponent<RectTransform>();
            tooltip.anchoredPosition = Vector2.MoveTowards(tooltip.anchoredPosition, new Vector2(targetPosition.x, tooltip.anchoredPosition.y), 7f);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator StartHover()
    {
        int position = 0;

        while (position != 20)
        {
            if (position < 20)
            {
            _cardImage.rectTransform.anchoredPosition += new Vector2(0, 3);
            RectTransform tooltip = _cardImage.gameObject.transform.parent.Find("Tooltip").GetComponent<RectTransform>();
            tooltip.anchoredPosition += new Vector2(0, 3);
            position += 3;
            }
            else if (position > 20)
            {
                _cardImage.rectTransform.anchoredPosition += new Vector2(0, -1);
                RectTransform tooltip = _cardImage.gameObject.transform.parent.Find("Tooltip").GetComponent<RectTransform>();
                tooltip.anchoredPosition += new Vector2(0, -1);
                position--;
            }    

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator CancelHover()
    {
        int position = 20;

        while (position != 0)
        {
            if (position > 0)
            {
                _cardImage.rectTransform.anchoredPosition += new Vector2(0, -3);
                RectTransform tooltip = _cardImage.gameObject.transform.parent.Find("Tooltip").GetComponent<RectTransform>();
                tooltip.anchoredPosition += new Vector2(0, -3);
                position -= 3;
            }
            else if (position < 0)
            {
                _cardImage.rectTransform.anchoredPosition += new Vector2(0, 1);
                RectTransform tooltip = _cardImage.gameObject.transform.parent.Find("Tooltip").GetComponent<RectTransform>();
                tooltip.anchoredPosition += new Vector2(0, 1);
                position++;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}