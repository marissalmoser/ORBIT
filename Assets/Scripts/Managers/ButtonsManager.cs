// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - October 16th 2024
// @Description - Manages the arrow controls
// +-------------------------------------------------------+
using UnityEngine;
public class ButtonsManager : MonoBehaviour
{
    #region Singleton
    private static ButtonsManager instance;
    public static ButtonsManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType(typeof(ButtonsManager)) as ButtonsManager;
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    int currentIndex;
    int maxIndex;
    GameManager _gameManager;
    UIManager _uiManager;
    public string currentCursor;
    void Start()
    {
        currentIndex = -1;
        maxIndex = 1;
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
    }

    #region Arrows
    public void ChangeMaxIndex(int maxIndex)
    {
        this.maxIndex = maxIndex;
    }

    public void ResetIndex()
    {
        currentIndex = -1;
    }

    public void IncreaseIndex()
    {
        //Connects the end of the list to the start of the list
        if (currentIndex >= maxIndex - 1)
            currentIndex = 0;
        else
            currentIndex++; //Increases index by 1

        _gameManager.currentlyOnWild = false;
        _uiManager.SetConfirmCard(currentIndex);
    }

    /// <summary>
    /// Decreases the index the card is showing
    /// If the index is already at the minimum value, set the index to the max value
    /// </summary>
    public void DecreaseIndex()
    {
        if (currentIndex <= 0) //Connects the start of the list to the end of the list
            currentIndex = maxIndex - 1;
        else
            currentIndex--; //Decreases index by 1

        _gameManager.currentlyOnWild = false;
        _uiManager.SetConfirmCard(currentIndex);
    }

    public int GetIndex() { return currentIndex;  }
    #endregion
}