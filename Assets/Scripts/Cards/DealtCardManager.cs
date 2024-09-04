// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 4 2024
// @Description - Manages the dealt cards
// +-------------------------------------------------------+

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DealtCardManager : MonoBehaviour
{

    //Makes Class a Singleton Class
    #region Singleton
    private static DealtCardManager instance;
    public static DealtCardManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(DealtCardManager)) as DealtCardManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    //Declares Variables
    [SerializeField] BoxCollider2D playArea;

    GameManager gameManager;
    Vector3 mousePosition;
    Vector3 imageStartingPosition;
    BoxCollider2D imageCollider;

    [SerializeField] PlayerInput playerInput;

    /// <summary>
    /// Initializes variables for DealtCardManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        gameManager = GameManager.Instance;
        mousePosition = Vector3.zero;
        imageStartingPosition = Vector3.zero;
    }

    /// <summary>
    /// Called when the mouse is pressed on a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void MousePressedCard(Image cardImage)
    {
        //If Game is ready for you to choose another card, allow card movement
        if (gameManager.gameState == GameManager.STATE.ChooseCards)
        {
            //Sets where the image originally was
            imageStartingPosition = cardImage.rectTransform.position;
            //Sets the mouse position
            mousePosition = Input.mousePosition;

            cardImage.enabled = true;
        }
    }

    /// <summary>
    /// Called when the mouse is released on a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    /// <param name="ID">The ID of the card</param>
    public void MouseReleasedCard(Image cardImage, int ID)
    {
        //If Game is ready for you to choose another card, allow card movement
        if (gameManager.gameState == GameManager.STATE.ChooseCards)
        {
            imageCollider = cardImage.GetComponent<BoxCollider2D>();
            //Checks if the image is overlapping with the play area
            if (imageCollider.IsTouching(playArea))
            {
                Destroy(cardImage.gameObject);
                gameManager.PlayCard(ID);
            }
            //Reset card position
            cardImage.rectTransform.position = imageStartingPosition;
            cardImage.enabled = false;
        }
    }

    /// <summary>
    /// Called when the mouse is pressed down and then moved on a dealt card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void OnDragCard(Image cardImage)
    {
        //If Game is ready for you to choose another card, allow card movement
        if (gameManager.gameState == GameManager.STATE.ChooseCards)
        {
            //Moves card image relative to mouse movements
            cardImage.transform.position = cardImage.transform.position - (mousePosition - Input.mousePosition);
            mousePosition = Input.mousePosition;
        }
    }
}