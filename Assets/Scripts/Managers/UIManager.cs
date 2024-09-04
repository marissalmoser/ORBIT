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
        for (int i = 0; i < dealtCardImages.Count; i++)
        {
            Destroy(dealtCardImages[i].gameObject);
        }
        dealtCardImages = new();

        List<Card> dealtCards = gameManager.GetDealtCards();
        int numOfDealtCards = dealtCards.Count;

        float cardWidth = dealtCardImage.rectTransform.rect.width;
        for (int i = 0; i < numOfDealtCards; i++)
        {
            Image newImage = Instantiate(dealtCardImage, Vector3.zero, Quaternion.identity);
            newImage.transform.SetParent(canvas.transform, false);
            newImage.rectTransform.anchoredPosition = new Vector3( (cardWidth + cardWidthSpacing ) * i + widthPadding, 0, 0);
            newImage.GetComponentInChildren<CardDisplay>().ID = i;

            dealtCardImages.Add(newImage);

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>();

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
        for (int i = 0; i < playedCardImages.Count; i++)
        {
            Destroy(playedCardImages[i].gameObject);
        }
        playedCardImages = new();

        List<Card> playedCards = gameManager.GetPlayedCards();
        int numOfPlayedCards = playedCards.Count;

        for (int i = 0; i < numOfPlayedCards; i++)
        {
            Image newImage = Instantiate(playedCardImage, Vector3.zero, Quaternion.identity);
            newImage.transform.SetParent(canvas.transform, false);
            newImage.rectTransform.anchoredPosition = new Vector3(-widthPadding, -cardHeightSpacing * i - heightPadding, 0);
            newImage.GetComponentInChildren<CardDisplay>().ID = i;

            playedCardImages.Add(newImage);
            print(newImage.GetComponentInChildren<CardDisplay>().ID);

            CardDisplay card = newImage.GetComponentInChildren<CardDisplay>();
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