// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 4 2024
// @Description - Manages the UI for the game
// +-------------------------------------------------------+

using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    #region Singleton
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(UIManager)) as UIManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [Header("Cards")]
    [SerializeField] private Image _cardBack;
    [SerializeField] private Sprite _cardBackSprite;
    [SerializeField] private Image _dealtCardImage, _playedCardImage, _turnLeftImage, _turnRightImage;
    [SerializeField] private int _widthPadding, _heightPadding;
    [SerializeField] private int _dealtCardWidthSpacing, _playedCardWidthSpacing, _cardHeightSpacing;
    [SerializeField] private bool doVerticalFormat;

    [Header("Tooltip")]
    [SerializeField] private Sprite _movedTooltip;
    [SerializeField] private Sprite _jumpTooltip, _turnTooltip, _switchTooltip, _clearTooltip;

    [Header("Folders")]
    [SerializeField] private Transform _dealtCardsFolder;
    [SerializeField] private Transform _playedCardsFolder;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _collectablesCount;
    [SerializeField] private TextMeshProUGUI _deckCount;
    [SerializeField] private Image _confirmButton, _cancelButton;

    [Header("Dealt Scriptable Objects")]
    [SerializeField] private Card _dealtMoveCard;
    [SerializeField] private Card _dealtJumpCard;
    [SerializeField] private Card _dealtTurnCard;
    [SerializeField] private Card _dealtTurnLeftCard;
    [SerializeField] private Card _dealtTurnRightCard;
    [SerializeField] private Card _dealtSwitchCard;
    [SerializeField] private Card _dealtClearCard;
    [SerializeField] private Card _backToItCard;

    [Header("Played Scriptable Objects")]
    [SerializeField] private Card _playedMoveCard;
    [SerializeField] private Card _playedJumpCard;
    [SerializeField] private Card _playedTurnLeftCard;
    [SerializeField] private Card _playedTurnRightCard;

    private GameManager _gameManager;

    private List<Image> _dealtCardImages;
    private List<Image> _playedCardImages;
    private Image _deckImage;

    private float _screenWidth, _screenHeight;

    private float cardWidth, cardHeight;

    /// <summary>
    /// Initializes variables for UIManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        _gameManager = _gameManager = GameManager.Instance;

        _screenWidth = _canvas.GetComponent<RectTransform>().rect.width;
        _screenHeight = _canvas.GetComponent<RectTransform>().rect.height;

        _dealtCardImages = new();
        _playedCardImages = new();

        cardWidth = _dealtCardImage.rectTransform.rect.width;
        cardHeight = _dealtCardImage.rectTransform.rect.height;

        _confirmButton.enabled = false;
        _cancelButton.enabled = false;
    }

    /// <summary>
    /// Updates the dealt cards in the UI
    /// </summary>
    public void UpdateDealtCards()
    {
        //Destroys all previous instances of dealt card images
        for (int i = 0; i < _dealtCardImages.Count; i++)
        {
            if (_dealtCardImages[i] != null)
                Destroy(_dealtCardImages[i].gameObject);
        }
        if (_deckImage != null)
            Destroy(_deckImage.gameObject); //Destroys deck image

        //Resets list
        _dealtCardImages = new();

        //Gets all dealt cards
        List<Card> dealtCards = _gameManager.GetDealtCards();
        int numOfDealtCards = dealtCards.Count;

        //Instantiates Card Back
        _deckImage = Instantiate(_cardBack, Vector3.zero, Quaternion.identity); //Instantiates new card;
        _deckImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
        _deckImage.rectTransform.anchoredPosition = new Vector3(_widthPadding, cardHeight + 20, 0); //Sets position
        _deckImage.sprite = _cardBackSprite;
        
        _deckCount.transform.SetAsFirstSibling();
        _deckImage.transform.SetAsFirstSibling();

        _deckCount.text = _gameManager._deck.Count.ToString();

        for (int i = 0; i < numOfDealtCards; i++)
        {
            Image newImage = Instantiate(_dealtCardImage, Vector3.zero, Quaternion.identity); //Instantiates new card
            newImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
            newImage.rectTransform.anchoredPosition = new Vector3( (cardWidth + _dealtCardWidthSpacing ) * i + _widthPadding, 0, 0); //Sets position
            newImage.GetComponentInChildren<CardDisplay>().ID = i; //Sets ID
            newImage.enabled = false; //Sets highlight to off

            //Makes tooltip invisible
            newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
            newImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            //If it is the first tooltip, off center it to keep it on screen
            if (i == 0)
                newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position = 
                    new Vector2(newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x + 25, 
                    newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);

            //Adds card into folder
            newImage.gameObject.transform.SetParent(_dealtCardsFolder);

            _dealtCardImages.Add(newImage); //Adds instantiated image to list

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Gets data from image

            //Finds the name and sets the image to the found data
            switch (dealtCards[i].name)
            {
                case Card.CardName.Move:
                    card.UpdateCard(_dealtMoveCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _movedTooltip;
                    break;
                case Card.CardName.Jump:
                    card.UpdateCard(_dealtJumpCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _jumpTooltip;
                    break;
                case Card.CardName.Turn:
                    card.UpdateCard(_dealtTurnCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT OR RIGHT.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _turnTooltip;
                    break;
                case Card.CardName.TurnLeft: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_dealtTurnLeftCard);
                    break;
                case Card.CardName.TurnRight: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_dealtTurnRightCard);
                    break;
                case Card.CardName.Clear:
                    card.UpdateCard(_dealtClearCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "REMOVES ONE CARD FROM ACTION ORDER.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _clearTooltip;
                    break;
                case Card.CardName.Switch:
                    card.UpdateCard(_dealtSwitchCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "SWAP TWO CARDS IN ACTION ORDER.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _switchTooltip;
                    break;
                case Card.CardName.BackToIt:
                    card.UpdateCard(_backToItCard);
                    break;
                default:
                    print("ERROR: COULD NOT UPDATE CARD IN UI");
                    break;
            }
        }
    }

    /// <summary>
    /// Updates the played cards in the UI
    /// </summary>
    public void UpdatePlayedCards()
    {
        //Destroys all previous instantiations of played cards
        for (int i = 0; i < _playedCardImages.Count; i++)
        {
            if (_playedCardImages[i] != null)
                Destroy(_playedCardImages[i].gameObject);
        }
        //Resets list
        _playedCardImages = new();

        //Gets all played cards
        List<Card> playedCards = _gameManager.GetPlayedCards();
        int numOfPlayedCards = playedCards.Count;

        for (int i = 0; i < numOfPlayedCards; i++)
        {
            Image newImage = Instantiate(_playedCardImage, Vector3.zero, Quaternion.identity); //Instantiates image
            newImage.transform.SetParent(_canvas.transform, false); //Sets canvas as the parent

            if (doVerticalFormat)
                newImage.rectTransform.anchoredPosition = new Vector3(-_widthPadding, _screenHeight - cardHeight / 2 -_cardHeightSpacing * i - _heightPadding, 0); //Sets position - Vertical Format
            else
                newImage.rectTransform.anchoredPosition = new Vector3((-_screenWidth / 2 + cardWidth / 2) - (_playedCardWidthSpacing * numOfPlayedCards / 2) + (_playedCardWidthSpacing * i + _widthPadding), -_heightPadding, 0); //Sets position - Horizontal Format
            
            newImage.GetComponentInChildren<CardDisplay>().ID = i; //Sets ID
            newImage.enabled = false; //Turns off highlight

            //Makes tooltip invisible
            newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
            newImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            //If it is the first tooltip, off center it to keep it on screen
            if (i == 0)
                newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                    new Vector2(newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x,
                    newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y - 20);

            //Adds card to folder
            newImage.gameObject.transform.SetParent(_playedCardsFolder);

            _playedCardImages.Add(newImage); //Adds image to list

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Grabs data from image
                                                                               //Uses grabbed data to compare with possible types and convert image to found typ
            switch (playedCards[i].name)
            {
                case Card.CardName.Move:
                    card.UpdateCard(_dealtMoveCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _movedTooltip;
                    break;
                case Card.CardName.Jump:
                    card.UpdateCard(_dealtJumpCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _jumpTooltip;
                    break;
                case Card.CardName.Turn: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_dealtTurnCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT OR RIGHT.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _turnTooltip;
                    break;
                case Card.CardName.TurnLeft:
                    card.UpdateCard(_dealtTurnLeftCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _turnTooltip;
                    break;
                case Card.CardName.TurnRight:
                    card.UpdateCard(_dealtTurnRightCard);
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT RIGHT.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _turnTooltip;
                    break;
                case Card.CardName.Clear: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_dealtClearCard);
                    break;
                case Card.CardName.Switch: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_dealtSwitchCard);
                    break;
                case Card.CardName.BackToIt:
                    card.UpdateCard(_backToItCard);
                    break;
                default:
                    print("ERROR: COULD NOT UPDATE CARD IN UI");
                    break;
            }
        }
    }

    //Initializes helper variable
    Image _confirmationImage;
    /// <summary>
    /// Creates a new instance of a card when a card is placed into the play area
    /// </summary>
    public void UpdateConfirmCard()
    {
        //Makes sure a clear or switch card was not played when it wasn't supposed to be played
        if (_gameManager.confirmationCard != null)
        {
            _confirmButton.enabled = true;
            _cancelButton.enabled = true;

            Card card = _gameManager.GetLastPlayedCard();

            //ERROR CHECK - They should already be deleted. If they haven't for whatever reason, delete them
            if (_confirmationImage != null)
                Destroy(_confirmationImage.gameObject);

            _confirmationImage = Instantiate(_dealtCardImage, Vector3.zero, Quaternion.identity); //Instantiates image
            _confirmationImage.transform.SetParent(_canvas.transform, false); //Sets canvas as the parent

            _confirmationImage.rectTransform.anchoredPosition = new Vector2(_screenWidth - 12 - cardWidth, 20);

            _confirmationImage.enabled = false; //Turns off highlight

            //Makes tooltip invisible
            _confirmationImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
            _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            _confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                        new Vector2(_confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x - 25,
                        _confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);

            CardDisplay cardDisplay = _confirmationImage.GetComponentInChildren<CardDisplay>(); //Grabs data from image
            switch (card.name)
            {
                case Card.CardName.Move:
                    cardDisplay.UpdateCard(_dealtMoveCard);
                    _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                    break;
                case Card.CardName.Jump:
                    cardDisplay.UpdateCard(_dealtJumpCard);
                    _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                    break;
                case Card.CardName.Turn: //Error Case. Should not be used, but it can be used if needed
                    cardDisplay.UpdateCard(_dealtTurnCard);
                    _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT OR RIGHT.";
                    break;
                case Card.CardName.TurnLeft:
                    cardDisplay.UpdateCard(_dealtTurnLeftCard);
                    _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT";
                    break;
                case Card.CardName.TurnRight:
                    cardDisplay.UpdateCard(_dealtTurnRightCard);
                    _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT RIGHT.";
                    break;
                case Card.CardName.Clear:
                    cardDisplay.UpdateCard(_dealtClearCard);
                    _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "REMOVES ONE CARD FROM ACTION ORDER.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _clearTooltip;
                    break;
                case Card.CardName.Switch:
                    cardDisplay.UpdateCard(_dealtSwitchCard);
                    _confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "SWAP TWO CARDS IN ACTION ORDER.";
                    //newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().sprite = _switchTooltip;
                    break;
                case Card.CardName.BackToIt:
                    cardDisplay.UpdateCard(_backToItCard);
                    break;
                default:
                    print("ERROR: COULD NOT UPDATE CARD IN UI");
                    break;
            }
        }
    }

    /// <summary>
    /// Destroys the confirm card
    /// </summary>
    public void DestroyConfirmCard()
    {
        _confirmButton.enabled = false;
        _cancelButton.enabled = false;

        if (_confirmationImage != null)
            Destroy(_confirmationImage.gameObject);
    }

    /// <summary>
    /// Updates how many collectables the player has attained
    /// </summary>
    public void UpdateCollectables()
    {
        int numOfCollectables = _gameManager.GetCollectableCount();
        _collectablesCount.text = numOfCollectables.ToString();
    }

    //Initialzes helper variable
    private Image _leftImage, _rightImage;
    /// <summary>
    /// Creates interactable Turn Cards to select which direction the player will turn
    /// </summary>
    public void CreateTurnCards()
    {
        //ERROR CHECK - They should already be deleted. If they haven't for whatever reason, delete them
        if (_leftImage != null)
            Destroy(_leftImage.gameObject);
        if (_rightImage != null)
            Destroy(_rightImage.gameObject);

        _leftImage = Instantiate(_turnLeftImage, Vector3.zero, Quaternion.identity); //Instantiates new card
        _leftImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
        _leftImage.rectTransform.anchoredPosition = new Vector2(_screenWidth - cardWidth * 4 - _dealtCardWidthSpacing, 0); //Sets position
        _rightImage = Instantiate(_turnRightImage, Vector3.zero, Quaternion.identity); //Instantiates new card
        _rightImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
        _rightImage.rectTransform.anchoredPosition = new Vector2(_screenWidth - cardWidth * 3, 0); //Sets position

        CardDisplay leftCard = _leftImage.GetComponent<CardDisplay>(); //Grabs data from image

        //Uses grabbed data to compare with possible types and convert image to found type
        leftCard.UpdateCard(_dealtTurnLeftCard);

        CardDisplay rightCard = _rightImage.GetComponent<CardDisplay>(); //Grabs data from image

        //Uses grabbed data to compare with possible types and convert image to found type
        rightCard.UpdateCard(_dealtTurnRightCard);
    }

    /// <summary>
    /// Destroys the turn cards after one is selected
    /// </summary>
    /// <param name="wasTurnLeftChosen">If the left turn card was selected or not</param>
    public void DestroyTurnCards(bool wasTurnLeftChosen)
    {
        //Destroys game objects
        if (_leftImage != null)
            Destroy(_leftImage.gameObject);
        if (_rightImage != null)
            Destroy(_rightImage.gameObject);

        //Player is turning left
        if (wasTurnLeftChosen)
        {
            _gameManager.AddTurnCard(_playedTurnLeftCard, true);
        }
        //Player is turning right
        else
        {
            _gameManager.AddTurnCard(_playedTurnRightCard, false);
        }
    }

    /// <summary>
    /// Returns the instantiated dealt card images
    /// </summary>
    /// <returns>List<Image> a list of images containing the instantiated dealt cards</returns>
    public List<Image> GetInstantiatedDealtCardImages() { return _dealtCardImages; }

    public List<Image> GetInstantiatedPlayedCardImages() {  return _playedCardImages; }
}