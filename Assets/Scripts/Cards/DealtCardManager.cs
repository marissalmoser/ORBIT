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

    //Initializes variables for CardManager. Called by GameManager
    public void Init()
    {
        gameManager = GameManager.Instance;
        mousePosition = Vector3.zero;
        imageStartingPosition = Vector3.zero;
    }

    /**
     * Called when the mouse is pressed on a dealt card
     */
    public void MousePressedCard(Image cardImage)
    {
        //Sets where the image originally was
        imageStartingPosition = cardImage.rectTransform.position;
        //Sets the mouse position
        mousePosition = Input.mousePosition;
    }

    /**
     * Called when the mouse is released on a dealt card
     */
    public void MouseReleasedCard(Image cardImage, int ID)
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
    }

    //Called when the mouse is pressed down and is moved on a dealt card
    public void OnDragCard(Image cardImage)
    {
        cardImage.transform.position = cardImage.transform.position - (mousePosition - Input.mousePosition);
        mousePosition = Input.mousePosition;
    }
}