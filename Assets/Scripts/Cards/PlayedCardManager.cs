// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - September 4 2024
// @Description - Manages the played cards
// +-------------------------------------------------------+

using System.Collections.Generic;
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

    /// <summary>
    /// Initializes variables for PlayedCardManager. Called by GameManager
    /// </summary>
    public void Init()
    {
        gameManager = GameManager.Instance;
    }

    /// <summary>
    /// Called when the mouse is pressed on a played card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    /// <param name="ID">The ID of the card</param>
    public void MousePressedCard(Image cardImage, int ID)
    {
        cardImage.enabled = true;
        //If Cards are being cleared
        if (gameManager.gameState == GameManager.STATE.ChooseClear)
        {
            gameManager.ClearAction(ID); //Calls method to take the card off of action order

            //Destroys game object
            Destroy(cardImage.gameObject);
        }

        //If Cards are being switched
        else if (gameManager.gameState == GameManager.STATE.SwitchCards)
        {
            gameManager.SwitchActionHelper(ID); //Calls the method helper to swap two cards' order
        }
    }

    /// <summary>
    /// Called when the mouse is released on a played card
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void MouseReleasedCard(Image cardImage)
    {
        if (gameManager.gameState != GameManager.STATE.SwitchCards) //If the player is not switching cards, remove highlight immediately
        {
            cardImage.enabled = false;
        }
        else
        {
            List<int> switchIDs = gameManager.GetCollectedSwitchIDs(); //If card has not been selected to be switched

            if (!switchIDs.Contains(cardImage.GetComponentInChildren<CardDisplay>().ID))
                cardImage.enabled = false;
        }
    }

    /// <summary>
    /// Called when the mouse enters the card's bounds
    /// </summary>
    /// <param name="cardImage">The image of the card</param>
    public void MouseEnterCard(Image cardImage)
    {
        //TODO
    }
}