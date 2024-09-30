// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 9 2024
// @Description - Manages the dealt cards
// +-------------------------------------------------------+

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class CardManager : MonoBehaviour
{
    #region Singleton
    private static CardManager instance;
    public static CardManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(CardManager)) as CardManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    //Declares Variables
    [SerializeField] private BoxCollider2D _playArea;

    private GameManager _gameManager;
    private UIManager _uiManager;
    private Vector3 _mousePosition;
    private Vector3 _imageStartingPosition;
    private BoxCollider2D _imageCollider;

    [NonSerialized] public Image clearCard;
    [NonSerialized] public (Image, Image) switchCards;
    [NonSerialized] public Image lastConfirmationCard;
    [NonSerialized] public bool isShowingDeck;

    /// <summary>
    /// Initializes variables for DealtCardManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        _mousePosition = Vector3.zero;
        _imageStartingPosition = Vector3.zero;

        lastConfirmationCard = null;
        isShowingDeck = false;
        lastConfirmationCard = null;
    }

    public void RemoveAllHighlight(List<Image> cards)
    {
        foreach (var card in cards)
        {
            card.enabled = false;
        }
    }

    #region Deck Methods

    public void MousePressedDeck()
    {

    }

    public void MouseReleasedDeck()
    {
        //Toggle
        isShowingDeck = !isShowingDeck;
        _uiManager.ShowDeck(isShowingDeck);
        

    }
    #endregion

    #region Dealt Card Methods

    /// <summary>
    /// Called when the mouse enters a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void DealtMouseEnterCard(Image toolTip)
    {
        //Makes tooltip visible
        if (!toolTip.GetComponentInParent<BoxCollider2D>().GetComponentInChildren<CardDisplay>().IsMouseDown) //Guaranteed to find parent with unique component
        {
            toolTip.enabled = true;
            toolTip.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        }
    }

    /// <summary>
    /// Called when the mouse exits a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void DealtMouseExitCard(Image toolTip)
    {
        //Makes tooltip invisible
        toolTip.enabled = false;
        toolTip.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }

    /// <summary>
    /// Called when the mouse is pressed on a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void DealtMousePressedCard(Image cardImage)
    {
        //sound call
        SfxManager.Instance.PlaySFX(3541);
        //Makes tooltip invisible
        cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = false;
        cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        //If Game is ready for you to choose another card, allow card movement
        if (_gameManager.gameState == GameManager.STATE.ChooseCards || _gameManager.gameState == GameManager.STATE.ConfirmCards
            || _gameManager.gameState == GameManager.STATE.ChooseTurn)
        {
            //Sets where the image originally was
            _imageStartingPosition = cardImage.rectTransform.position;
            //Sets the mouse position
            _mousePosition = Input.mousePosition;

            cardImage.transform.SetAsLastSibling(); //Makes sure other card's tooltips do not appear

            cardImage.enabled = true;
        }
    }

    /// <summary>
    /// Called when the mouse is released on a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    /// <param name="ID">The ID of the card</param>
    public void DealtMouseReleasedCard(Image cardImage, int ID)
    {
        //sound effect call
        SfxManager.Instance.PlaySFX(1092);
        //Makes tooltip visible
        if (cardImage.GetComponentInChildren<CardDisplay>().IsMouseInCard)
        {
            cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = true;
            cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        }

        //If Game is ready for you to choose another card, allow card movement
        if (_gameManager.gameState == GameManager.STATE.ChooseCards || _gameManager.gameState == GameManager.STATE.ConfirmCards
            || _gameManager.gameState == GameManager.STATE.ChooseTurn)
        {
            _imageCollider = cardImage.GetComponent<BoxCollider2D>();
            //Checks if the image is overlapping with the play area
            if (_imageCollider.IsTouching(_playArea) 
                && (cardImage.GetComponentInChildren<CardDisplay>().card.name != Card.CardName.Clear || _gameManager.GetPlayedCards().Count != 0)
                && (cardImage.GetComponentInChildren<CardDisplay>().card.name != Card.CardName.Switch || _gameManager.GetPlayedCards().Count > 1))
            {
                //Resets GameManager variables ( in case card was replaced with a different one )
                _gameManager.isClearing = false;
                _gameManager.isSwitching = false;
                _gameManager.isTurning = false;
                switchCards.Item1 = null;
                switchCards.Item2 = null;
                clearCard = null;

                //Erases switch and clear sprites from playedCards
                List<Image> tempPlayedCards = _uiManager.GetInstantiatedPlayedCardImages();
                int tempCount = tempPlayedCards.Count;
                for (int i = 0; i < tempCount; i++)
                {
                    tempPlayedCards[i].gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
                    tempPlayedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
                }

                //Disables confirmation button
                _uiManager.confirmButton.GetComponent<ConfirmationControls>().isActive = false;

                //If the player was choosing a turn card when it got replaced
                if (_gameManager.gameState == GameManager.STATE.ChooseTurn)
                {
                    //Turns off the darken effect
                    if (_gameManager.lowerDarkenIndex)
                    {
                        _gameManager.darken.transform.SetSiblingIndex(_gameManager.darken.transform.GetSiblingIndex() - 1);
                    }
                    _gameManager.lowerDarkenIndex = false;

                    //Destroys the turn cards
                    _uiManager.DestroyTurnCards();
                }

                //If a card was replaced
                if (lastConfirmationCard != null)
                {
                    //Respawns previous card
                    lastConfirmationCard.gameObject.SetActive(true);
                    lastConfirmationCard.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = false;
                    lastConfirmationCard.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                    _gameManager.StopDemo();
                }

                lastConfirmationCard = cardImage;

                cardImage.gameObject.SetActive(false);
                _gameManager.PlayCard(ID);
            }
            //Reset card position
            cardImage.rectTransform.position = _imageStartingPosition;
            cardImage.enabled = false;
        }
    }

    /// <summary>
    /// Called when the mouse is pressed down and then moved on a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void DealtOnDragCard(Image cardImage)
    {
        //If Game is ready for you to choose another card, allow card movement
        if (_gameManager.gameState == GameManager.STATE.ChooseCards || _gameManager.gameState == GameManager.STATE.ConfirmCards
            || _gameManager.gameState == GameManager.STATE.ChooseTurn)
        {
            //Moves card image relative to mouse movements
            cardImage.transform.position = cardImage.transform.position - (_mousePosition - Input.mousePosition);
            _mousePosition = Input.mousePosition;
        }
    }
    #endregion

    #region Played Card Methods
    /// <summary>
    /// Called when the mouse is pressed on a played card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    /// <param name="ID">The ID of the card</param>
    public void PlayedMousePressedCard(Image cardImage)
    {
        //sound effect call
        SfxManager.Instance.PlaySFX(8885);

        //Makes tooltip invisible
        cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = false;
        cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        cardImage.enabled = true;
    }

    /// <summary>
    /// Called when the mouse is released on a played card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void PlayedMouseReleasedCard(Image cardImage)
    {
        List<Image> playedCards = _uiManager.GetInstantiatedPlayedCardImages();
        bool isCopy = false;

        //Sets the card to clear
        if (_gameManager.isClearing)
            clearCard = cardImage;

        //Sets the cards to switch
        if (_gameManager.isSwitching)
        {
            //Checks if the card is already in item 1, if it is, deletes it from switchCards
            if (switchCards.Item1 != null && cardImage.GetComponentInChildren<CardDisplay>().ID == switchCards.Item1.GetComponentInChildren<CardDisplay>().ID)
            {
                switchCards.Item1 = null;
                isCopy = true;

                //If there was an element in Item2, push it to item1
                if (switchCards.Item2 != null)
                {
                    switchCards.Item1 = switchCards.Item2;
                    switchCards.Item2 = null;
                }
            }
            //Checks if the card is already in item 2, if it is, deletes it from switchCards
            if (switchCards.Item2 != null && cardImage.GetComponentInChildren<CardDisplay>().ID == switchCards.Item2.GetComponentInChildren<CardDisplay>().ID)
            {
                switchCards.Item2 = null;
                isCopy = true;
            }

            //If the card was not already in switchCards, adds it in
            if (!isCopy)
            {
                if (switchCards.Item1 == null)
                    switchCards.Item1 = cardImage;
                else if (switchCards.Item2 == null)
                    switchCards.Item2 = cardImage;
                else
                {
                    //Replaced oldest card for the most recent card clicked
                    switchCards.Item1 = switchCards.Item2;
                    switchCards.Item2 = cardImage;
                }
            }
        }

        //Makes tooltip visible
        if (cardImage.GetComponentInChildren<CardDisplay>().IsMouseInCard)
        {
            cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = true;
            cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        }

        if (!_gameManager.isSwitching) //If the player is not switching cards, remove highlight immediately
        {
            cardImage.enabled = false;
        }
        else
        {
            List<int> switchIDs = _gameManager.GetCollectedSwitchIDs(); //If card has not been selected to be switched

            if (!switchIDs.Contains(cardImage.GetComponentInChildren<CardDisplay>().ID))
            {
                cardImage.enabled = false;
            }
        }

        int size = playedCards.Count;
        for (int i = 0; i < size; i++)
        {
            //Handles which card is being cleared
            if (_gameManager.isClearing)
            {
                if (playedCards[i].GetComponentInChildren<CardDisplay>().ID != clearCard.GetComponentInChildren<CardDisplay>().ID)
                    playedCards[i].gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
            }

            //Handles which card is being swapped
            if (_gameManager.isSwitching)
            {
                if (switchCards.Item1 != null && switchCards.Item2 == null)
                {
                    //If first value is not null but second value is null, only check first value
                    if (playedCards[i].GetComponentInChildren<CardDisplay>().ID != switchCards.Item1.GetComponentInChildren<CardDisplay>().ID)
                        playedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
                }
                else if (switchCards.Item1 == null && switchCards.Item2 != null)
                {
                    //If first value is null but second value is not null, only check first value
                    if (playedCards[i].GetComponentInChildren<CardDisplay>().ID != switchCards.Item2.GetComponentInChildren<CardDisplay>().ID)
                        playedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
                }
                else if (switchCards.Item1 != null && switchCards.Item2 != null)
                {
                    //If neither value is null, check both values
                    if (playedCards[i].GetComponentInChildren<CardDisplay>().ID != switchCards.Item1.GetComponentInChildren<CardDisplay>().ID
                        && playedCards[i].GetComponentInChildren<CardDisplay>().ID != switchCards.Item2.GetComponentInChildren<CardDisplay>().ID)
                        playedCards[i].gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
                }
            }
        }

        //If Cards are being cleared
        if (_gameManager.isClearing)
        {
            _gameManager.ClearAction(); //Calls method to take the card off of action order
        }

        //If Cards are being switched
        else if (_gameManager.isSwitching)
        {
            _gameManager.SwitchAction();
        }

        if (_gameManager.isClearing && clearCard != null)
        {
            _uiManager.confirmButton.GetComponent<ConfirmationControls>().isActive = true;
            _uiManager.cancelButton.GetComponent<ConfirmationControls>().isActive = true;
        }
        if (_gameManager.isSwitching && switchCards.Item1 != null && switchCards.Item2 != null)
        {
            _uiManager.confirmButton.GetComponent<ConfirmationControls>().isActive = true;
            _uiManager.cancelButton.GetComponent<ConfirmationControls>().isActive = true;
        }

    }

    /// <summary>
    /// Called when the mouse enters the card's bounds
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void PlayedMouseEnterCard(Image cardImage)
    {
        if (_gameManager.isClearing)
            cardImage.gameObject.transform.Find("Clear").GetComponent<Image>().enabled = true;
        if (_gameManager.isSwitching)
            cardImage.gameObject.transform.Find("Swap").GetComponent<Image>().enabled = true;

        //Makes tooltip visible
        cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = true;
        cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        //cardImage.GetComponentInParent<Canvas>().overrideSorting = true;
        //cardImage.GetComponent<Canvas>().sortingOrder = 1;
    }

    /// <summary>
    /// Called when the mouse leaves the card's bounds
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void PlayedMouseExitCard(Image cardImage)
    {
        //Removes clear highlight
        if (_gameManager.isClearing)
        {
            //If clearCard is null, no card has been selected
            if (clearCard == null)
                cardImage.gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
            //Checks clear card and see if it matches
            else if (cardImage.GetComponentInChildren<CardDisplay>().ID != clearCard.GetComponentInChildren<CardDisplay>().ID)
                cardImage.gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
        }
        //If game state is not in clearing, remove all highlight
        else
            cardImage.gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;

        //Removes swap highlight
        if (_gameManager.isSwitching)
        {
            //If both values are null, nothing is selected
            if (switchCards.Item1 == null && switchCards.Item2 == null)
                cardImage.gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
            else if (switchCards.Item1 != null && switchCards.Item2 == null)
            {
                //If first value is not null but second value is null, only check first value
                if (cardImage.GetComponentInChildren<CardDisplay>().ID != switchCards.Item1.GetComponentInChildren<CardDisplay>().ID)
                    cardImage.gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
            }
            else if (switchCards.Item1 == null && switchCards.Item2 != null)
            {
                //If first value is null but second value is not null, check first value
                if (cardImage.GetComponentInChildren<CardDisplay>().ID != switchCards.Item2.GetComponentInChildren<CardDisplay>().ID)
                    cardImage.gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
            }
            else if (switchCards.Item1 != null && switchCards.Item2 != null)
            {
                //If neither value is null, check both values
                if (cardImage.GetComponentInChildren<CardDisplay>().ID != switchCards.Item1.GetComponentInChildren<CardDisplay>().ID
            && cardImage.GetComponentInChildren<CardDisplay>().ID != switchCards.Item2.GetComponentInChildren<CardDisplay>().ID)
                    cardImage.gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
            }
        }
        else
        {
            cardImage.gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;
        }

        //Makes tooltip invisible
        cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = false;
        cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        //cardImage.GetComponentInParent<Canvas>().overrideSorting = true;
        //cardImage.GetComponentInParent<Canvas>().sortingOrder = 0;
    }

    /// <summary>
    /// Called when the player inputs to turn left
    /// </summary>
    public void PlayedTurnChooseLeft()
    {
        //sound effect call
        SfxManager.Instance.PlaySFX(8885);
        _uiManager.DestroyTurnCards(true);
    }

    /// <summary>
    /// Called when the player inputs to turn right
    /// </summary>
    public void PlayedTurnChooseRight()
    {
        //sound effect call
        SfxManager.Instance.PlaySFX(8885);
        _uiManager.DestroyTurnCards(false);
    }

    /// <summary>
    /// Makes tooltip visible when mouse enters card
    /// </summary>
    /// <param name="tooltip"></param>
    public void MouseEnterTurnCard(Image tooltip)
    {
        tooltip.enabled = true;
        tooltip.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    /// <summary>
    /// Makes tooltip invisible when mouse exits card
    /// </summary>
    /// <param name="tooltip"></param>
    public void MouseExitTurnCard(Image tooltip)
    {
        tooltip.enabled = false;
        tooltip.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }
    #endregion
}