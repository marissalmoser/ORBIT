using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayedCardManager : MonoBehaviour
{

    //Makes Class a Singleton Class
    #region Singleton
    private static PlayedCardManager instance;
    public static PlayedCardManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(PlayedCardManager)) as PlayedCardManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    GameManager gameManager;

    [SerializeField] PlayerInput playerInput;

    //Initializes variables for CardManager. Called by GameManager
    public void Init()
    {
        gameManager = GameManager.Instance;
    }

    /**
     * Called when the mouse is pressed on a dealt card
     */
    public void MousePressedCard(Image cardImage)
    {
        //If Cards are being cleared
        if (gameManager.gameState == GameManager.STATE.ChooseClear)
        {


        }

        //If Cards are being switched
        if (gameManager.gameState == GameManager.STATE.RunActionOrder)
        {

        }

        //Return to game loop
        gameManager.ChangeGameState(GameManager.STATE.RunActionOrder);
    }
}