// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 4 2024
// @Description - Manages the UI for the game
// +-------------------------------------------------------+

using System.Collections.Generic;
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
    [SerializeField] Image dealtCardImage, playedCardImage;
    [SerializeField] private int widthPadding, heightPadding;
    [SerializeField] private int cardWidthSpacing, cardHeightSpacing;
    [SerializeField] GameObject canvas;

    [SerializeField] Card moveCard;
    [SerializeField] Card jumpCard;
    [SerializeField] Card turnCard;
    [SerializeField] Card switchCard;
    [SerializeField] Card clearCard;
    [SerializeField] Card backToItCard;

    private GameManager gameManager;

    List<Image> dealtCardImages;
    List<Image> playedCardImages;

    /// <summary>
    /// Initializes variables for UIManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        gameManager = gameManager = GameManager.Instance;

        dealtCardImages = new();
        playedCardImages = new();
    }

    /// <summary>
    /// Updates the dealt cards in the UI
    /// </summary>
    public void UpdateDealtCards()
    {
        //Destroys all previous instances of dealt card images
        for (int i = 0; i < dealtCardImages.Count; i++)
        {
            if (dealtCardImages[i] != null)
                Destroy(dealtCardImages[i].gameObject);
        }
        //Resets list
        dealtCardImages = new();

        //Gets all dealt cards
        List<Card> dealtCards = gameManager.GetDealtCards();
        int numOfDealtCards = dealtCards.Count;

        float cardWidth = dealtCardImage.rectTransform.rect.width; //Gets width of a card
        for (int i = 0; i < numOfDealtCards; i++)
        {
            Image newImage = Instantiate(dealtCardImage, Vector3.zero, Quaternion.identity); //Instantiates new card
            newImage.transform.SetParent(canvas.transform, false); //Sets canvas as its parent
            newImage.rectTransform.anchoredPosition = new Vector3( (cardWidth + cardWidthSpacing ) * i + widthPadding, 0, 0); //Sets position
            newImage.GetComponentInChildren<CardDisplay>().ID = i; //Sets ID
            newImage.enabled = false; //Sets highlight to off
            dealtCardImages.Add(newImage); //Adds instantiated image to list

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Gets data from image

            //Finds the name and sets the image to the found data
            switch (dealtCards[i].name)
            {
                case Card.CardName.Move:
                    card.UpdateCard(moveCard);
                    break;
                case Card.CardName.Jump:
                    card.UpdateCard(jumpCard);
                    break;
                case Card.CardName.Turn:
                    card.UpdateCard(turnCard);
                    break;
                case Card.CardName.Clear:
                    card.UpdateCard(clearCard);
                    break;
                case Card.CardName.Switch:
                    card.UpdateCard(switchCard);
                    break;
                case Card.CardName.BackToIt:
                    card.UpdateCard(backToItCard);
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
        for (int i = 0; i < playedCardImages.Count; i++)
        {
            if (playedCardImages[i] != null)
                Destroy(playedCardImages[i].gameObject);
        }
        //Resets list
        playedCardImages = new();

        //Gets all played cards
        List<Card> playedCards = gameManager.GetPlayedCards();
        int numOfPlayedCards = playedCards.Count;

        for (int i = 0; i < numOfPlayedCards; i++)
        {
            Image newImage = Instantiate(playedCardImage, Vector3.zero, Quaternion.identity); //Instantiates image
            newImage.transform.SetParent(canvas.transform, false); //Sets canvas as the parent
            newImage.rectTransform.anchoredPosition = new Vector3(-widthPadding, -cardHeightSpacing * i - heightPadding, 0); //Sets position
            newImage.GetComponentInChildren<CardDisplay>().ID = i; //Sets ID
            newImage.enabled = false; //Turns off highlight
            playedCardImages.Add(newImage); //Adds image to list

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>(); //Grabs data from image
            //Uses grabbed data to compare with possible types and convert image to found type
            switch (playedCards[i].name)
            {
                case Card.CardName.Move:
                    card.UpdateCard(moveCard);
                    break;
                case Card.CardName.Jump:
                    card.UpdateCard(jumpCard);
                    break;
                case Card.CardName.Turn:
                    card.UpdateCard(turnCard);
                    break;
                case Card.CardName.Clear:
                    card.UpdateCard(clearCard);
                    break;
                case Card.CardName.Switch:
                    card.UpdateCard(switchCard);
                    break;
                case Card.CardName.BackToIt:
                    card.UpdateCard(backToItCard);
                    break;
                default:
                    print("ERROR: COULD NOT UPDATE CARD IN UI");
                    break;
            }
        }
    }

    /// <summary>
    /// Returns the instantiated dealt card images
    /// </summary>
    /// <returns>List<Image> a list of images containing the instantiated dealt cards</returns>
    public List<Image> GetInstantiatedDealtCardImages() { return dealtCardImages; }

    public List<Image> GetInstantiatedPlayedCardImages() {  return playedCardImages; }
}