// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - October 16th 2024
// @Description - Manages arrow functionality
// +-------------------------------------------------------+
using UnityEngine;

public class ArrowControls : MonoBehaviour
{
    private ArrowsManager _arrowsManager;
    void Start()
    {
        _arrowsManager = ArrowsManager.Instance;
    }

    /// <summary>
    /// Increases the index of the card showing
    /// If the index is already at the highest value, set it to 0
    /// </summary>
    public void IncreaseIndex()
    {
        _arrowsManager.IncreaseIndex();
    }

    /// <summary>
    /// Decreases the index the card is showing
    /// If the index is already at the minimum value, set the index to the max value
    /// </summary>
    public void DecreaseIndex()
    {
        _arrowsManager.DecreaseIndex();
    }
}
