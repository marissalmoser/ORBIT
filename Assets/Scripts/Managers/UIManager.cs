// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - November 21st 2024
// @Description - Manages the UI for the game
// +-------------------------------------------------------+

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

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
    [SerializeField] private Image _deckImage;
    [SerializeField] private Image _deckShownImage, _dealtCardImage, _playedCardImage, _confirmCardImage, _turnLeftImage, _turnRightImage;
    [SerializeField] private int _widthPadding, _heightPadding;
    [SerializeField] private int _dealtCardWidthSpacing, _playedCardWidthSpacing, _cardHeightSpacing;
    [SerializeField] private bool doVerticalFormat;
    public int numOfUniqueCards = 7;

    [Header("Index Buttons")]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [Header("Tooltip")]
    [SerializeField] private Sprite _movedTooltip;
    [SerializeField] private Sprite _jumpTooltip, _turnTooltip, _switchTooltip, _clearTooltip;
    [SerializeField] private Image _upperTextBox;

    [Header("Folders")]
    [SerializeField] private RectTransform _dealtCardsFolder;
    [SerializeField] private RectTransform _playedCardsFolder;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _deckCount;
    [SerializeField] private RectTransform cardSlot;
    private Vector2 _deckCountPos;
    public Button confirmButton, cancelButton;

    [Header("Deck Scriptable Objects")]
    [SerializeField] private Card _deckCard;

    [Header("Card Scriptable Objects")]
    [SerializeField] private Card _moveCard;
    [SerializeField] private Card _jumpCard;
    [SerializeField] private Card _turnCard;
    [SerializeField] private Card _turnLeftCard;
    [SerializeField] private Card _turnRightCard;
    [SerializeField] private Card _switchCard;
    [SerializeField] private Card _clearCard;
    [SerializeField] private Card _stallCard;
    [SerializeField] private Card _wildCard;

    private GameManager _gameManager;
    private ButtonsManager _buttonsManager;
    private CardManager _cardManager;

    private Image _deck;
    public Image confirmationImage;
    private List<Image> _shownDeck;
    private List<Image> _dealtCardImages;
    private List<Image> _playedCardImages;

    public float screenWidth, screenHeight;

    [NonSerialized] public float cardWidth;
    private float _cardHeight;

    private Vector2 _nextPlayCardPosition;

    [NonSerialized] public Card confirmCard;

    public int shiftIndex;
    private int _numOfCardsToAddToDeck;
    private List<Image> movedImages;

    /// <summary>
    /// Initializes variables for UIManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        _gameManager = GameManager.Instance;
        _buttonsManager = ButtonsManager.Instance;
        _cardManager = CardManager.Instance;

        screenWidth = _canvas.GetComponent<RectTransform>().rect.width;
        screenHeight = _canvas.GetComponent<RectTransform>().rect.height;

        _shownDeck = new();
        _dealtCardImages = new();
        _playedCardImages = new();

        cardWidth = _dealtCardImage.rectTransform.rect.width;
        _cardHeight = _dealtCardImage.rectTransform.rect.height;

        //Disables buttons on start 
        confirmButton.GetComponent<ButtonControls>().SetIsActive(false);
        cancelButton.GetComponent<ButtonControls>().SetIsActive(false);

        _leftButton.gameObject.SetActive(false);
        _rightButton.gameObject.SetActive(false);

        _upperTextBox.enabled = false;
        _upperTextBox.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        _nextPlayCardPosition = new Vector2(-_widthPadding, screenHeight - _cardHeight / 2 - _heightPadding);

        _deckCountPos = _deckCount.GetComponent<RectTransform>().anchoredPosition;
        _deckCount.enabled = false;
        cardSlot.anchoredPosition = new Vector2(-_widthPadding, -_heightPadding);

        shiftIndex = 0;
        _numOfCardsToAddToDeck = 0;
        movedImages = new();
    }

    public void StartDeckAnim()
    {
        DeckInstantiation(new Vector2(_widthPadding, -400));
        _deckCount.enabled = true;
        StartCoroutine(MoveDeckOntoScreen(new Vector2(_widthPadding, _heightPadding)));
    }

    IEnumerator MoveDeckOntoScreen(Vector2 targetPos)
    {
        while (_deck.rectTransform.anchoredPosition != targetPos)
        {
            Vector2 moveDelta = Vector2.MoveTowards(_deck.rectTransform.anchoredPosition, new Vector3(targetPos.x, targetPos.y), 15f * Time.deltaTime * 60f);

            _deckCount.rectTransform.anchoredPosition += moveDelta - _deck.rectTransform.anchoredPosition; //Moves deckCount by the amount the deck itself moved
            _deck.rectTransform.anchoredPosition = moveDelta; //Moves the deck

            yield return new WaitForEndOfFrame();
        }
        GameManager.Instance.ChangeGameState(GameManager.STATE.ChooseCards);
    }

    /// <summary>
    /// Updates the dealt cards in the UI
    /// </summary>
    public void UpdateDealtCards(List<Card> dealtCards)
    {
        //Destroys all previous instances of dealt card images
        for (int i = 0; i < _dealtCardImages.Count; i++)
        {
            if (_dealtCardImages[i] != null)
            {
                Destroy(_dealtCardImages[i].gameObject);
            }
        }

        if (_deck != null)
            Destroy(_deck.gameObject); //Destroys deck image

        //Resets list
        _dealtCardImages = new();

        int numOfDealtCards = dealtCards.Count;

        DeckInstantiation(new Vector3(_widthPadding, _heightPadding, 0));

        //Instantiates and sets up Cards
        for (int i = 0; i < numOfDealtCards; i++)
        {
            DealtCardInstantiation(dealtCards[i], i);
        }
    }

    /// <summary>
    /// Begins moving a card
    /// </summary>
    /// <param name="card">Card to be moved</param>
    /// <param name="index">The position where the card should go</param>
    /// <param name="numOfCards">The amount of cards that are being dealt</param>
    public void StartMoveCardFromDeck(Card card, int index, int numOfCards)
    {
        _numOfCardsToAddToDeck = numOfCards;
        StartCoroutine(MoveDealtCardFromDeck(card, index));
    }

    public void StartMoveCard(Image card, int index)
    {
        _numOfCardsToAddToDeck = _gameManager.GetDealtCards().Count;
        StartCoroutine(MoveDealtCard(card, index));
    }

    //Helper Variable
    int _currentCardsDone = 0;

    /// <summary>
    /// When coroutine is finished, this method is called
    /// </summary>
    private void FinishMovingCardsFromDeck()
    {
        _currentCardsDone++;
        if (_currentCardsDone == _numOfCardsToAddToDeck) //Checks if the current number of cards moved is equal to the total number of cards required to move
        {
            UpdateTextBox("DRAG A CARD TO PLAY.");
            UpdateDealtCards(_gameManager.GetDealtCards()); //Updates Cards
            CardManager.Instance.canMoveCard = true;
            _currentCardsDone = 0;

            for (int i = 0; i < movedImages.Count; i++)
            {
                Destroy(movedImages[i].gameObject);
            }
            movedImages = new();
        }
    }

    IEnumerator MoveDealtCardFromDeck(Card card, int index)
    {
        yield return new WaitForSeconds((_numOfCardsToAddToDeck - index) * 0.2f * Time.deltaTime * 60); // Waits for delay
        ReduceDeckCount(1);

        Vector2 targetPos = new Vector3((cardWidth + _dealtCardWidthSpacing) * (index + 1) + _widthPadding, _heightPadding);
        Image tempImage = DealtCardInstantiation(card, index);

        tempImage.rectTransform.anchoredPosition = new Vector2(_widthPadding, _heightPadding);

        while (tempImage.rectTransform.anchoredPosition != targetPos)
        {
            tempImage.rectTransform.anchoredPosition = Vector2.MoveTowards(tempImage.rectTransform.anchoredPosition, 
                targetPos, 20f * Time.deltaTime * 60f); //Moves the deck

            yield return new WaitForEndOfFrame();
        }

        //Adds image to List, so that it can be destroyed later
        movedImages.Add(tempImage);
        FinishMovingCardsFromDeck();
        yield return null;
    }

    IEnumerator MoveDealtCard(Image card, int index)
    {
        yield return new WaitForSeconds(index * 0.05f * Time.deltaTime * 60); // Waits for delay

        Vector2 targetPos = new Vector3((cardWidth + _dealtCardWidthSpacing) * (index + 1) + _widthPadding, _heightPadding);

        while (card.rectTransform.anchoredPosition != targetPos)
        {
            card.rectTransform.anchoredPosition = Vector2.MoveTowards(card.rectTransform.anchoredPosition,
                targetPos, 20f * Time.deltaTime * 90f); //Moves the deck

            yield return new WaitForEndOfFrame();
        }

        FinishMovingCardsFromDeck();
        yield return null;
    }

    /// <summary>
    /// Instantiates the deck
    /// </summary>
    /// <param name="position">The position to put the deck</param>
    private void DeckInstantiation(Vector2 position)
    {
        _deck = Instantiate(_deckImage, Vector2.zero, Quaternion.identity);
        _deck.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
        _deck.rectTransform.anchoredPosition = position; //Sets position

        CardDisplay deckCard = _deck.GetComponentInChildren<CardDisplay>(); //Gets data from image
        deckCard.card = _deckCard;
        _deckCount.enabled = true;

        _deckCount.GetComponent<RectTransform>().anchoredPosition = (position - new Vector2(_widthPadding, _heightPadding)) + _deckCountPos;
        _deckCount.text = _gameManager.deck.Count.ToString();
        if (_gameManager.deck.Count > 1)
        {
            deckCard.isFromWild = true;
        }
        else
        {
            deckCard.isFromWild = false;
            _deckCount.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 39.4f);

            if (_gameManager.deck.Count == 0) //Repositions the 0, since it is off center
            {
                _deckCount.GetComponent<RectTransform>().anchoredPosition -= new Vector2(10.9f, 0f);
                _deckCount.text = "O"; //The 0 in the font doesn't look good. Changes it to a capital O
            }
        }

        deckCard.SetImage();
        _deck.transform.SetSiblingIndex(8);
        _deckCount.transform.SetSiblingIndex(9);
    }

    private void ReduceDeckCount(int amount)
    {
        SfxManager.Instance.PlaySFX(5429);
        int deckSize;
        if (_deckCount.text != "O")
        {
            deckSize = int.Parse(_deckCount.text);
            deckSize -= amount;
            _deckCount.text = deckSize.ToString();
        }
        else
            deckSize = 0;
        CardDisplay deckDisplay = _deck.GetComponentInChildren<CardDisplay>();
        _deckCount.GetComponent<RectTransform>().anchoredPosition = (_deck.rectTransform.anchoredPosition - new Vector2(_widthPadding, _heightPadding)) + _deckCountPos;
        if (deckSize > 1)
        {
            deckDisplay.isFromWild = true;
        }
        else
        {
            deckDisplay.isFromWild = false;
            _deckCount.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 39.4f);

            if (_gameManager.deck.Count == 0) //Repositions the 0, since it is off center
            {
                _deckCount.GetComponent<RectTransform>().anchoredPosition -= new Vector2(10.9f, 0f);
                _deckCount.text = "O"; //The 0 in the font doesn't look good. Changes it to a capital O
            }
        }
        deckDisplay.SetImage();
    }

    /// <summary>
    /// Instantiates the dealt card
    /// </summary>
    /// <param name="card">the card to instantiate</param>
    /// <param name="index">the index of the card</param>
    private Image DealtCardInstantiation(Card card, int index)
    {
        Image newImage = Instantiate(_dealtCardImage, Vector3.zero, Quaternion.identity); //Instantiates new card
        newImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
        newImage.rectTransform.anchoredPosition = new Vector3((cardWidth + _dealtCardWidthSpacing)
            * (index + 1) + _widthPadding, _heightPadding, 0); //Sets position
        newImage.GetComponentInChildren<CardDisplay>().ID = index; //Sets ID
        newImage.enabled = false; //Sets highlight to off

        //Makes tooltip invisible
        newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
        newImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        //If it is the first tooltip, off center it to keep it on screen
        if (index == 0)
            newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                new Vector2(newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x + 25,
                newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);

        //Adds card into folder
        newImage.gameObject.transform.SetParent(_dealtCardsFolder);

        //newImage.GetComponentInChildren<CardDisplay>().isDarken = isDarkenList[i];

        _dealtCardImages.Add(newImage); //Adds instantiated image to list

        CardDisplay cardDisplay = newImage.GetComponentInChildren<CardDisplay>(); //Gets data from image

        //TODO - Put in function when you have time - UpdateCard(Card.name)
        //Finds the name and sets the image to the found data
        switch (card.name)
        {
            case Card.CardName.Move:
                cardDisplay.card = _moveCard;
                newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                break;
            case Card.CardName.Jump:
                cardDisplay.card = _jumpCard;
                newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                break;
            case Card.CardName.Turn:
                cardDisplay.card = _turnCard;
                newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT OR RIGHT.";
                break;
            case Card.CardName.TurnLeft: //Error Case. Should not be used, but it can be used if needed
                cardDisplay.card = _turnLeftCard;
                break;
            case Card.CardName.TurnRight: //Error Case. Should not be used, but it can be used if needed
                cardDisplay.card = _turnRightCard;
                break;
            case Card.CardName.Clear:
                cardDisplay.card = _clearCard;
                newImage.GetComponentInChildren<TextMeshProUGUI>().text = "REMOVES ONE CARD FROM ACTION ORDER.";
                break;
            case Card.CardName.Switch:
                cardDisplay.card = _switchCard;
                newImage.GetComponentInChildren<TextMeshProUGUI>().text = "SWAP TWO CARDS IN ACTION ORDER.";
                break;
            case Card.CardName.Stall:
                cardDisplay.card = _stallCard;
                newImage.GetComponentInChildren<TextMeshProUGUI>().text = "REPEAT ACTION ORDER WITHOUT ADDING ANY CARD.";
                break;
            case Card.CardName.Wild:
                cardDisplay.card = _wildCard;
                newImage.GetComponentInChildren<TextMeshProUGUI>().text = "CHOOSE ANY CARD TO PUT INTO THE ACTION ORDER.";
                break;
            default:
                print("ERROR: COULD NOT UPDATE CARD IN UI");
                break;
        }
        return newImage;
    }

    /// <summary>
    /// Updates the played cards in the UI
    /// </summary>
    public void UpdatePlayedCards(List<Card> playedCards)
    {
        //Destroys all previous instantiations of played cards
        List<bool> tempIsWildList = new();

        for (int i = 0; i < _playedCardImages.Count; i++)
        {
            if (_playedCardImages[i] != null)
            {
                tempIsWildList.Add(_playedCardImages[i].GetComponentInChildren<CardDisplay>().isFromWild);
                Destroy(_playedCardImages[i].gameObject);
            }
            else
            {
                tempIsWildList.Add(false);
            }
        }

        //Adds confirm card into list
        if (_confirmationDisplay != null)
        {
            tempIsWildList.Add(_confirmationDisplay.isFromWild);
        }

        //Resets list
        _playedCardImages = new();

        //Instantiates card images
        int numOfPlayedCards = playedCards.Count;

        for (int i = 0; i < numOfPlayedCards; i++)
        {
            Image newImage = Instantiate(_playedCardImage, Vector3.zero, Quaternion.identity); //Instantiates image
            newImage.transform.SetParent(_canvas.transform, false); //Sets canvas as the parent

            if (doVerticalFormat)
                newImage.rectTransform.anchoredPosition = new Vector2(-_widthPadding - _playedCardWidthSpacing *
                    (shiftIndex - i), screenHeight - _cardHeight / 2 - _heightPadding); //Sets position - Vertical Format
            else
                newImage.rectTransform.anchoredPosition = new Vector2((-screenWidth / 2 + cardWidth / 2) - (_playedCardWidthSpacing * numOfPlayedCards / 2)
                    + (_playedCardWidthSpacing * i + _widthPadding), -_heightPadding); //Sets position - Horizontal Format
            
            if (i == numOfPlayedCards - 1)
            {
                //Gets next card position
                _nextPlayCardPosition = new Vector2(-_widthPadding, screenHeight - _cardHeight / 2 - _cardHeightSpacing * (i + 1) - _heightPadding);
            }

            newImage.GetComponentInChildren<CardDisplay>().ID = i; //Sets ID
            newImage.enabled = false; //Turns off highlight

            //Makes tooltip invisible
            newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
            newImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            //Makes clear and switch hover invisible
            newImage.gameObject.transform.Find("Clear").GetComponent<Image>().enabled = false;
            newImage.gameObject.transform.Find("Swap").GetComponent<Image>().enabled = false;

            //If it is the first tooltip, off center it to keep it on screen
            if (i == numOfPlayedCards - 1)
                newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                   new Vector2(newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x - 50,
                   newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);

            //Adds card to folder
            newImage.gameObject.transform.SetParent(_playedCardsFolder);

            _playedCardImages.Add(newImage); //Adds image to list

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Grabs data from image
                                                                               //Uses grabbed data to compare with possible types and convert image to found type
            card.isFromWild = tempIsWildList[i];
            switch (playedCards[i].name)
            {
                case Card.CardName.Move:
                    card.card = _moveCard;
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                    break;
                case Card.CardName.Jump:
                    card.card = _jumpCard;
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                    break;
                case Card.CardName.Turn: //Error Case. Should not be used, but it can be used if needed
                    card.card = _turnCard;
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT OR RIGHT.";
                    break;
                case Card.CardName.TurnLeft:
                    card.card = _turnLeftCard;
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT";
                    break;
                case Card.CardName.TurnRight:
                    card.card = _turnRightCard;
                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT RIGHT.";
                    break;
                case Card.CardName.Clear: //Error Case. Should not be used, but it can be used if needed
                    card.card = _clearCard;
                    break;
                case Card.CardName.Switch: //Error Case. Should not be used, but it can be used if needed
                    card.card = _switchCard;
                    break;
                case Card.CardName.Stall:
                    card.card = _stallCard;
                    break;
                case Card.CardName.Wild:
                    card.card = _wildCard;
                    break;
                default:
                    print("ERROR: COULD NOT UPDATE CARD IN UI");
                    break;
            }
        }
    }

    //Initializes helper variable
    CardDisplay _confirmationDisplay;
    /// <summary>
    /// Creates a new instance of a card when a card is placed into the play area
    /// </summary>
    public void UpdateConfirmCard()
    {
        if (_gameManager.isTurning)
            _buttonsManager.ChangeMaxIndex(2);
        else
            _buttonsManager.ChangeMaxIndex(numOfUniqueCards);
        //Makes sure a clear or switch card was not played when it wasn't supposed to be played
        if (_gameManager.confirmationCard != null)
        {
            confirmCard = _gameManager.GetLastPlayedCard();

            bool tempIsWild = false;
            //ERROR CHECK - They should already be deleted. If they haven't for whatever reason, delete them
            if (confirmationImage != null)
            {
                tempIsWild = confirmationImage.GetComponentInChildren<CardDisplay>().isFromWild;
                Destroy(confirmationImage.gameObject);
            }

            confirmationImage = Instantiate(_confirmCardImage, Vector3.zero, Quaternion.identity); //Instantiates image
            confirmationImage.transform.SetParent(_canvas.transform, false); //Sets canvas as the parent

            confirmationImage.rectTransform.anchoredPosition = new Vector2(screenWidth - 12 - cardWidth, 20);

            confirmationImage.enabled = false; //Turns off highlight

            //Makes tooltip invisible
            confirmationImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
            confirmationImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                        new Vector2(confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x - 25,
                        confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);

            _confirmationDisplay = confirmationImage.GetComponentInChildren<CardDisplay>(); //Grabs data from image

            _confirmationDisplay.isFromWild = tempIsWild;
            switch (confirmCard.name)
            {
                case Card.CardName.Move:
                    _confirmationDisplay.card = _moveCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                    break;
                case Card.CardName.Jump:
                    _confirmationDisplay.card = _jumpCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                    break;
                case Card.CardName.Turn: //Error Case. Should not be used, but it can be used if needed
                    _confirmationDisplay.card = _turnCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT OR RIGHT.";
                    break;
                case Card.CardName.TurnLeft:
                    _confirmationDisplay.card = _turnLeftCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT";
                    break;
                case Card.CardName.TurnRight:
                    _confirmationDisplay.card = _turnRightCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS RIGHT.";
                    break;
                case Card.CardName.Clear:
                    _confirmationDisplay.card = _clearCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "REMOVES ONE CARD FROM ACTION ORDER.";
                    break;
                case Card.CardName.Switch:
                    _confirmationDisplay.card = _switchCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "SWAP TWO CARDS IN ACTION ORDER.";
                    break;
                case Card.CardName.Stall:
                    _confirmationDisplay.card = _stallCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "REPEAT ACTION ORDER WITHOUT ADDING ANY CARD.";
                    break;
                case Card.CardName.Wild:
                    _confirmationDisplay.card = _wildCard;
                    confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "CHOOSE ANY CARD TO PUT INTO THE ACTION ORDER.";
                    break;
                default:
                    print("ERROR: COULD NOT UPDATE CARD IN UI");
                    break;
            }
        }
    }

    /// <summary>
    /// Changes the confirm card to a different card determined by the index
    /// Main functionaility is to choose which card to play when Wild or Turn card is being played
    /// </summary>
    /// <param name="index">The card to switch the confirm card to</param>
    public void SetConfirmCard(int index)
    {
        //Makes sure a clear or switch card was not played when it wasn't supposed to be played
        if (_gameManager.confirmationCard != null)
        {
            Card card = _gameManager.GetLastPlayedCard();
            //ERROR CHECK - They should already be deleted. If they haven't for whatever reason, delete them
            if (confirmationImage != null)
                Destroy(confirmationImage.gameObject);

            confirmationImage = Instantiate(_confirmCardImage, Vector3.zero, Quaternion.identity); //Instantiates image
            confirmationImage.transform.SetParent(_canvas.transform, false); //Sets canvas as the parent

            confirmationImage.rectTransform.anchoredPosition = new Vector2(screenWidth - 12 - cardWidth, 20);

            confirmationImage.enabled = false; //Turns off highlight

            //Makes tooltip invisible
            confirmationImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
            confirmationImage.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                        new Vector2(confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x - 25,
                        confirmationImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);

            _confirmationDisplay = confirmationImage.GetComponentInChildren<CardDisplay>(); //Grabs data from image
            if (_gameManager.isTurning)
            {
                _confirmationDisplay.isDarken = false;
                _confirmationDisplay.isFromWild = false;
                switch (index)
                {
                    case 0:
                        //Sets the confirm card
                        _confirmationDisplay.card = _turnRightCard;
                        _gameManager.confirmationCard = _turnRightCard;
                        confirmCard = _turnRightCard;
                        //Sets gamestate to accomodate the new card
                        _gameManager.TurnAction(confirmCard);
                        _gameManager.RunPlaySequence();

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS RIGHT.";
                        break;
                    case 1:
                        //Sets the confirm card
                        _confirmationDisplay.card = _turnLeftCard;
                        _gameManager.confirmationCard = _turnLeftCard;
                        confirmCard = _turnLeftCard;

                        //Sets the gamestate to accomodate the new card
                        _gameManager.TurnAction(confirmCard);
                        _gameManager.RunPlaySequence();

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT";
                        break;
                }
            }
            else
            {
                _confirmationDisplay.isFromWild = true;
                switch (index)
                {
                    case 0:
                        //Sets the confirm card
                        _confirmationDisplay.card = _moveCard;
                        _gameManager.confirmationCard = _moveCard;
                        confirmCard = _moveCard;

                        //Sets gamestate to accomodate the new card
                        _gameManager.WildAction(confirmCard);
                        _gameManager.RunPlaySequence();

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                        break;
                    case 1:
                        //Sets the confirm card
                        _confirmationDisplay.card = _jumpCard;
                        _gameManager.confirmationCard = _jumpCard;
                        confirmCard = _jumpCard;

                        //Sets gamestate to accomodate the new card
                        _gameManager.WildAction(confirmCard);
                        _gameManager.RunPlaySequence();

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                        break;
                    case 2:
                        //Sets the confirm card
                        _confirmationDisplay.card = _turnLeftCard;
                        _gameManager.confirmationCard = _turnLeftCard;
                        confirmCard = _turnLeftCard;

                        //Sets the gamestate to accomodate the new card
                        _gameManager.WildAction(confirmCard);
                        _gameManager.RunPlaySequence();

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT";
                        break;
                    case 3:
                        //Sets the confirm card
                        _confirmationDisplay.card = _turnRightCard;
                        _gameManager.confirmationCard = _turnRightCard;
                        confirmCard = _turnRightCard;
                        //Sets gamestate to accomodate the new card
                        _gameManager.WildAction(confirmCard);
                        _gameManager.RunPlaySequence();

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "TURNS RIGHT.";
                        break;
                    case 4:
                        //Sets the confirm card
                        _confirmationDisplay.card = _clearCard;
                        _gameManager.confirmationCard = _clearCard;
                        confirmCard = _clearCard;

                        //Sets gamestate to accomodate the new card
                        _gameManager.WildAction(confirmCard);

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "REMOVES ONE CARD FROM ACTION ORDER.";
                        break;
                    case 5:
                        //Sets the confirm card
                        _confirmationDisplay.card = _switchCard;
                        _gameManager.confirmationCard = _switchCard;
                        confirmCard = _switchCard;

                        ////Sets gamestate to accomodate the new card
                        _gameManager.WildAction(confirmCard);

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "SWAP TWO CARDS IN ACTION ORDER.";
                        break;
                    case 6:
                        //Sets the confirm card
                        _confirmationDisplay.card = _stallCard;
                        _gameManager.confirmationCard = _stallCard;
                        confirmCard = _stallCard;

                        //Sets gamestate to accomodate the new card
                        _gameManager.WildAction(confirmCard);
                        _gameManager.RunPlaySequence();

                        //Updates UI
                        UpdatePlayedCards(_gameManager.GetPlayedCards());
                        confirmationImage.GetComponentInChildren<TextMeshProUGUI>().text = "REPEAT ACTION ORDER WITHOUT ADDING ANY CARD.";
                        break;
                    default:
                        print("ERROR: COULD NOT UPDATE CARD IN UI");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Destroys the confirm card
    /// </summary>
    public void DestroyConfirmCard()
    {
        //Disables buttons
        confirmButton.GetComponent<ButtonControls>().SetIsActive(false);
        cancelButton.GetComponent<ButtonControls>().SetIsActive(false);

        if (confirmationImage != null)
            Destroy(confirmationImage.gameObject);
    }

    /// <summary>
    /// Displays or hides the arrows to change the confirm card
    /// </summary>
    public void UpdateArrows()
    {
        //If the gamestate is on wild, show the arrows
        if (_gameManager.isUsingWild || _gameManager.isTurning)
        {
            _leftButton.gameObject.SetActive(true);
            _rightButton.gameObject.SetActive(true);
        }
        else
        {
            _leftButton.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(false);
        }
    }

    public void ShowDeck(bool isShowingDeck)
    {
        if (isShowingDeck)
        {
            //ERROR CHECK - This should already be done
            foreach (Image image in _shownDeck)
            {
                if (image != null)
                    Destroy(image.gameObject);
            }
            _gameManager.deckShownDarken.enabled = true;

            //Gets all dealt cards
            List<Card> startingCards = _gameManager.GetStartingCards();

            int numOfCardsToShow = 0;
            int[] instances = new int[numOfUniqueCards];

            //Gets the number of cards in the deck, for each card
            for (int j = 0; j < startingCards.Count; j++)
            {
                switch (startingCards[j].name)
                {
                    case Card.CardName.Move:
                        instances[0]++;
                        break;
                    case Card.CardName.Turn:
                        instances[1]++;
                        break;
                    case Card.CardName.Jump:
                        instances[2]++;
                        break;
                    case Card.CardName.Stall:
                        instances[3]++;
                        break;
                    case Card.CardName.Clear:
                        instances[4]++;
                        break;
                    case Card.CardName.Switch:
                        instances[5]++;
                        break;
                    case Card.CardName.Wild:
                        instances[6]++;
                        break;
                }
            }

            //Gets how many cards to show
            for (int i = 0; i < numOfUniqueCards; i++)
            {
                if (instances[i] > 0)
                    numOfCardsToShow++;
            }

            //Shows Cards
            int indexPosition = 0;
            for (int i = 0; i < numOfUniqueCards; i++)
            {
                if (instances[i] > 0)
                {
                    Image newImage = Instantiate(_deckShownImage, Vector3.zero, Quaternion.identity); //Instantiates new card
                    newImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent

                    //Centers Image Horizontally
                    if (numOfCardsToShow % 2 == 0) // Number of Cards to Show is Even
                    {
                        float x = screenWidth / 2;

                        if (indexPosition < numOfCardsToShow / 2)
                        {
                            newImage.rectTransform.anchoredPosition = new Vector3((cardWidth + _dealtCardWidthSpacing + 50) *
                                -(numOfCardsToShow / 2 - indexPosition) + x, 575, 0); //Sets position
                            
                        }
                        else
                        {
                            newImage.rectTransform.anchoredPosition = new Vector3((cardWidth + _dealtCardWidthSpacing + 50) *
                                ((indexPosition - numOfCardsToShow / 2)) + x, 575, 0); //Sets position
                        }
                    }
                    else // Number of Cards To Show is Odd
                    {
                        float x = screenWidth / 2 - cardWidth / 2;

                        if (i < numOfCardsToShow / 2)
                        {
                            newImage.rectTransform.anchoredPosition = new Vector3((cardWidth + _dealtCardWidthSpacing + 50) *
                                -(numOfCardsToShow / 2 - indexPosition) + x, 575, 0); //Sets position
                            if (i == 0 && numOfCardsToShow == 7) //Pushes tooltip onto screen
                                newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                                    new Vector2(newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x + 25,
                                    newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);
                        }
                        else if (indexPosition > numOfCardsToShow / 2)
                        {
                            newImage.rectTransform.anchoredPosition = new Vector3((cardWidth + _dealtCardWidthSpacing + 50) *
                                ((indexPosition - numOfCardsToShow / 2)) + x, 575, 0); //Sets position
                            if (i == 6 && numOfCardsToShow == 7) //Pushes tooltip onto screen
                                newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position =
                                    new Vector2(newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.x - 25,
                                    newImage.gameObject.transform.Find("Tooltip").gameObject.transform.position.y);
                        }
                        else
                        {
                            newImage.rectTransform.anchoredPosition = new Vector3((cardWidth + _dealtCardWidthSpacing + 50) * 0 +
                                x, 575, 0); //Sets position
                        }
                    }

                    _shownDeck.Add(newImage); //Adds instantiated image to list

                    newImage.GetComponentInChildren<TextMeshProUGUI>().text = instances[i].ToString();
                    newImage.gameObject.transform.Find("Tooltip").GetComponent<Image>().enabled = false;
                    newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                    //Shows Card Sprite
                    CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Gets data from image
                    switch (i)
                    {
                        case 0: //Move
                            card.card = _moveCard;
                            newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.";
                            break;
                        case 1: //Turn
                            card.card = _turnCard;
                            newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().text = "TURNS LEFT OR RIGHT.";
                            break;
                        case 2: //Jump
                            card.card = _jumpCard;
                            newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().text = "MOVE FORWARD ONE TILE.\nCAN JUMP TO HIGHER GROUND.";
                            break;
                        case 3: //Replay
                            card.card = _stallCard;
                            newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().text = "REPEAT ACTION ORDER WITHOUT ADDING ANY CARD.";
                            break;
                        case 4: //Clear
                            card.card = _clearCard;
                            newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().text = "REMOVES ONE CARD FROM ACTION ORDER.";
                            break;
                        case 5: //Switch
                            card.card = _switchCard;
                            newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().text = "SWAP TWO CARDS IN ACTION ORDER.";
                            break;
                        case 6: //Wild
                            card.card = _wildCard;
                            newImage.transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>().text = "CHOOSE ANY CARD TO PUT INTO THE ACTION ORDER.";
                            break;
                        default:
                            print("ERROR: COULD NOT UPDATE CARD IN UI");
                            break;
                    }
                }
                else
                {
                    //There are no instances of this card. Do not show it and reduce position index
                    indexPosition--;
                }
                indexPosition++;
            }
        }
        else
        {
            if (_gameManager.deckShownDarken != null)
                _gameManager.deckShownDarken.enabled = false;
            foreach (Image image in _shownDeck)
            {
                if (image != null)
                    Destroy(image.gameObject);
            }
        }
    }

    /// <summary>
    /// When confirm is clicked, moves the card from the play area to the action order
    /// </summary>
    public void MoveCardToActionOrder()
    {
        //StartCoroutine(MoveCard(confirmationImage, _nextPlayCardPosition.y));
        StartCoroutine(MoveCard(confirmationImage));
    }

    /// <summary>
    /// Updates the text box text
    /// </summary>
    public void UpdateTextBox(string text)
    {
        _upperTextBox.enabled = true;
        _upperTextBox.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        _upperTextBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// Disables the text box and its text
    /// </summary>
    public void DisableTextBox()
    {
        _upperTextBox.enabled = false;
        _upperTextBox.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        _upperTextBox.GetComponentInChildren<TextMeshProUGUI>().text = "null";
    }

    /// <summary>
    /// Returns the instantiated dealt card images
    /// </summary>
    /// <returns>List<Image> a list of images containing the instantiated dealt cards</returns>
    public List<Image> GetInstantiatedDealtCardImages() { return _dealtCardImages; }

    public List<Image> GetInstantiatedPlayedCardImages() {  return _playedCardImages; }

    public IEnumerator MoveCard(Image image)
    {
        Vector2 targetPosition = new Vector2(-_widthPadding, screenHeight - _cardHeight / 2 - _heightPadding);
        while (image.rectTransform.anchoredPosition.y != targetPosition.y)
        {
            //Moves card
            image.rectTransform.anchoredPosition = Vector2.MoveTowards(image.rectTransform.anchoredPosition, new Vector2(screenWidth - cardWidth / 2 - _widthPadding, targetPosition.y), 24f * Time.deltaTime * 60);

            //Shrinks x value
            if(image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta.x > _playedCardImage.rectTransform.sizeDelta.x)
            {
                image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta -= new Vector2(4f * Time.deltaTime * 60, 0);
                image.rectTransform.position += new Vector3(0.25f, 0);
                if (image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta.x < _playedCardImage.rectTransform.sizeDelta.x)
                    image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = new Vector2(_playedCardImage.rectTransform.sizeDelta.x, 
                        image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta.y);
            }

            //Shrinks Y value
            if (image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta.y > _playedCardImage.rectTransform.sizeDelta.y)
            {
                image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta -= new Vector2(0, 4f * Time.deltaTime * 60);
                image.rectTransform.position += new Vector3(0, 0.25f);
                if (image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta.y < _playedCardImage.rectTransform.sizeDelta.y)
                    image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = new Vector2(image.gameObject.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta.x, 
                        _playedCardImage.rectTransform.sizeDelta.y);
            }

            yield return new WaitForEndOfFrame();
        }
        UpdatePlayedCards(_gameManager.GetPlayedCards());
        _gameManager.PlaySequence();
        DestroyConfirmCard();
    }
}