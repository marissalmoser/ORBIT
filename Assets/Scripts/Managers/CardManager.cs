// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 9 2024
// @Description - Manages the dealt cards
// +-------------------------------------------------------+

using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    /// <summary>
    /// Initializes variables for DealtCardManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        _mousePosition = Vector3.zero;
        _imageStartingPosition = Vector3.zero;
    }

    public void RemoveAllHighlight(List<Image> cards)
    {
        foreach (var card in cards)
        {
            card.enabled = false;
        }
    }

    #region Dealt Card Methods

    /// <summary>
    /// Called when the mouse enters a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void DealtMouseEnterCard(Image toolTip)
    {
        //Makes tooltip visible
        toolTip.enabled = true;
        toolTip.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
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
        //SfxManager.Instance.PlaySFX(3541);
        //Makes tooltip invisible
        cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = false;
        cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        //If Game is ready for you to choose another card, allow card movement
        if (_gameManager.gameState == GameManager.STATE.ChooseCards)
        {
            //Sets where the image originally was
            _imageStartingPosition = cardImage.rectTransform.position;
            //Sets the mouse position
            _mousePosition = Input.mousePosition;

            cardImage.transform.SetAsLastSibling();

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
        if (_gameManager.gameState == GameManager.STATE.ChooseCards)
        {
            _imageCollider = cardImage.GetComponent<BoxCollider2D>();
            //Checks if the image is overlapping with the play area
            if (_imageCollider.IsTouching(_playArea))
            {
                Destroy(cardImage.gameObject);
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
        if (_gameManager.gameState == GameManager.STATE.ChooseCards)
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
    public void PlayedMousePressedCard(Image cardImage, int ID)
    {
        //sound effect call
        SfxManager.Instance.PlaySFX(8885);

        //Makes tooltip invisible
        cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = false;
        cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        cardImage.enabled = true;
        //If Cards are being cleared
        if (_gameManager._isClearing)
        {
            _gameManager.ClearAction(ID); //Calls method to take the card off of action order
        }

        //If Cards are being switched
        else if (_gameManager._isSwitching)
        {
            _gameManager.SwitchActionHelper(ID); //Calls the method helper to swap two cards' order
        }
    }

    /// <summary>
    /// Called when the mouse is released on a played card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void PlayedMouseReleasedCard(Image cardImage)
    {
        //Makes tooltip visible
        if (cardImage.GetComponentInChildren<CardDisplay>().IsMouseInCard)
        {
            cardImage.gameObject.transform.Find("Tooltip").gameObject.GetComponent<Image>().enabled = true;
            cardImage.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        }

        if (!_gameManager._isSwitching) //If the player is not switching cards, remove highlight immediately
        {
            cardImage.enabled = false;
        }
        else
        {
            List<int> switchIDs = _gameManager.GetCollectedSwitchIDs(); //If card has not been selected to be switched

            if (!switchIDs.Contains(cardImage.GetComponentInChildren<CardDisplay>().ID))
                cardImage.enabled = false;
        }
        if (_gameManager._isClearing)
        {
            cardImage.enabled = true;
        }
    }

    /// <summary>
    /// Called when the mouse enters the card's bounds
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void PlayedMouseEnterCard(Image cardImage)
    {
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
    #endregion
}