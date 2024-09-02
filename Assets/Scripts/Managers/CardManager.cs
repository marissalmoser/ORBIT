using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{

    //Makes Class a Singleton Class
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
    [SerializeField] BoxCollider2D playArea;

    GameManager gameManager;
    Vector3 mousePosition;
    Vector3 imageStartingPosition;
    Collider2D imageCollider;

    //Inits variables for CardManager. Called by GameManager
    public void Init()
    {
        print(playArea);
        gameManager = GameManager.Instance;
        mousePosition = Vector3.zero;
        imageStartingPosition = Vector3.zero;
    }

    /**
     * Called when the mouse is pressed on a dealt card
     */
    public void MousePressedCard(Image cardImage)
    {
        print("HERE");
        //Checks if the left mouse button was pressed down
        if (Input.GetMouseButtonDown(0))
        {
            //Sets where the image originally was
            imageStartingPosition = cardImage.transform.position;

            //Sets the mouse position
            mousePosition = Input.mousePosition;
        }
    }

    /**
     * Called when the mouse is released on a dealt card
     */
    public void MouseReleasedCard(Image cardImage)
    {
        //Checks if the left mouse button was released
        if (Input.GetMouseButtonUp(0))
        {
            imageCollider = cardImage.GetComponent<Collider2D>();
            //Checks if the image is overlapping with the play area
            print("IMAGE: " + imageCollider);
            print("AREA: " + playArea);
            if (imageCollider.IsTouching(playArea))
            {
                //TODO Play
                //gameManager.PlayCard();
            }
            //Reset card position
            cardImage.transform.position = imageStartingPosition;


        }
    }

    //Called when the mouse is pressed down and is moved on a dealt card
    public void OnDragCard(Image cardImage)
    {
        //Checks if the left mouse button is being held
        if (Input.GetMouseButton(0))
        {
            cardImage.transform.position = cardImage.transform.position - (mousePosition - Input.mousePosition);
            mousePosition = Input.mousePosition;
        }
    }
}