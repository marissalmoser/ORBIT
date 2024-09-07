// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 4 2024
// @Description - Manages the UI for the game
// +-------------------------------------------------------+

using System.Collections.Generic;
using UnityEditor.PackageManager;
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
    [SerializeField] private Image _dealtCardImage, _playedCardImage, _turnLeftImage, _turnRightImage;
    [SerializeField] private int _widthPadding, _heightPadding;
    [SerializeField] private int _cardWidthSpacing, _cardHeightSpacing;
    [SerializeField] private GameObject _canvas;

    [SerializeField] private Card _moveCard;
    [SerializeField] private Card _jumpCard;
    [SerializeField] private Card _turnCard;
    [SerializeField] private Card _turnLeftCard;
    [SerializeField] private Card _turnRightCard;
    [SerializeField] private Card _switchCard;
    [SerializeField] private Card _clearCard;
    [SerializeField] private Card _backToItCard;

    private GameManager _gameManager;

    private List<Image> _dealtCardImages;
    private List<Image> _playedCardImages;

    /// <summary>
    /// Initializes variables for UIManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        _gameManager = _gameManager = GameManager.Instance;

        _dealtCardImages = new();
        _playedCardImages = new();
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
        //Resets list
        _dealtCardImages = new();

        //Gets all dealt cards
        List<Card> dealtCards = _gameManager.GetDealtCards();
        int numOfDealtCards = dealtCards.Count;

        float cardWidth = _dealtCardImage.rectTransform.rect.width; //Gets width of a card
        for (int i = 0; i < numOfDealtCards; i++)
        {
            Image newImage = Instantiate(_dealtCardImage, Vector3.zero, Quaternion.identity); //Instantiates new card
            newImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
            newImage.rectTransform.anchoredPosition = new Vector3( (cardWidth + _cardWidthSpacing ) * i + _widthPadding, 0, 0); //Sets position
            newImage.GetComponentInChildren<CardDisplay>().ID = i; //Sets ID
            newImage.enabled = false; //Sets highlight to off
            _dealtCardImages.Add(newImage); //Adds instantiated image to list

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Gets data from image

            //Finds the name and sets the image to the found data
            switch (dealtCards[i].name)
            {
                case Card.CardName.Move:
                    card.UpdateCard(_moveCard);
                    break;
                case Card.CardName.Jump:
                    card.UpdateCard(_jumpCard);
                    break;
                case Card.CardName.Turn:
                    card.UpdateCard(_turnCard);
                    break;
                case Card.CardName.TurnLeft: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_turnLeftCard);
                    break;
                case Card.CardName.TurnRight: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_turnRightCard);
                    break;
                case Card.CardName.Clear:
                    card.UpdateCard(_clearCard);
                    break;
                case Card.CardName.Switch:
                    card.UpdateCard(_switchCard);
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
            newImage.rectTransform.anchoredPosition = new Vector3(-_widthPadding, -_cardHeightSpacing * i - _heightPadding, 0); //Sets position
            newImage.GetComponentInChildren<CardDisplay>().ID = i; //Sets ID
            newImage.enabled = false; //Turns off highlight
            _playedCardImages.Add(newImage); //Adds image to list

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Grabs data from image
            //Uses grabbed data to compare with possible types and convert image to found type
            switch (playedCards[i].name)
            {
                case Card.CardName.Move:
                    card.UpdateCard(_moveCard);
                    break;
                case Card.CardName.Jump:
                    card.UpdateCard(_jumpCard);
                    break;
                case Card.CardName.Turn: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_turnCard);
                    break;
                case Card.CardName.TurnLeft:
                    card.UpdateCard(_turnLeftCard);
                    break;
                case Card.CardName.TurnRight:
                    card.UpdateCard(_turnRightCard);
                    break;
                case Card.CardName.Clear: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_clearCard);
                    break;
                case Card.CardName.Switch: //Error Case. Should not be used, but it can be used if needed
                    card.UpdateCard(_switchCard);
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

    //Initialzes helper variable
    private Image leftTurnCard, rightTurnCard;
    private Image _leftImage, _rightImage;
    /// <summary>
    /// Creates interactable Turn Cards to select which direction the player will turn
    /// </summary>
    public void CreateTurnCards()
    {
            float cardWidth = _turnLeftImage.rectTransform.rect.width; //Gets width of a card
            float cardHeight = _turnLeftImage.rectTransform.rect.height;

            _leftImage = Instantiate(_turnLeftImage, Vector3.zero, Quaternion.identity); //Instantiates new card
            _leftImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
            _leftImage.rectTransform.anchoredPosition = new Vector3(_widthPadding, cardHeight + 20, 0); //Sets position

            _rightImage = Instantiate(_turnRightImage, Vector3.zero, Quaternion.identity); //Instantiates new card
            _rightImage.transform.SetParent(_canvas.transform, false); //Sets canvas as its parent
            _rightImage.rectTransform.anchoredPosition = new Vector3(_widthPadding + cardWidth + _cardWidthSpacing, cardHeight + 20, 0); //Sets position

            CardDisplay leftCard = _leftImage.GetComponent<CardDisplay>(); //Grabs data from image

            //Uses grabbed data to compare with possible types and convert image to found type
            leftCard.UpdateCard(_turnLeftCard);
            

            CardDisplay rightCard = _rightImage.GetComponent<CardDisplay>(); //Grabs data from image

            //Uses grabbed data to compare with possible types and convert image to found type
            rightCard.UpdateCard(_turnRightCard);
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
            _gameManager.AddToPlayedCards(_turnLeftCard);
        }
        //Player is turning right
        else
        {
            _gameManager.AddToPlayedCards(_turnRightCard);
        }
    }

    /// <summary>
    /// Returns the instantiated dealt card images
    /// </summary>
    /// <returns>List<Image> a list of images containing the instantiated dealt cards</returns>
    public List<Image> GetInstantiatedDealtCardImages() { return _dealtCardImages; }

    public List<Image> GetInstantiatedPlayedCardImages() {  return _playedCardImages; }
}