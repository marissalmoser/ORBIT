// +-------------------------------------------------------+
// @author - Ryan Herwig
// @Contributers - 
// @Last modified - October 16th 2024
// @Description - Manages arrow functionality
// +-------------------------------------------------------+
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonControls : MonoBehaviour
{
    private GameManager _gameManager;
    private UIManager _uiManager;
    private ArrowsManager _arrowsManager;
    private PauseMenu _pauseMenu;

    public bool isActive;
    private Animator _anim;
    public static Action CancelCard;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        _arrowsManager = ArrowsManager.Instance;
        _pauseMenu = FindAnyObjectByType<PauseMenu>();
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    #region Confirm / Cancel
    /// <summary>
    /// Setter used to set the confirm and cancel buttons along with their animations.
    /// should only trigger anim if state is changing
    /// </summary>
    /// <param name="input"></param>
    public void SetIsActive(bool input)
    {
        //ensures the bool is changing;
        if (input == isActive)
        {
            return;
        }

        //sets button to input state
        isActive = input;

        if (input)
        {
            _anim.SetTrigger("activeAnim");
            return;
        }

        _anim.SetTrigger("inactiveAnim");
    }

    public void ConfirmPressed()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && isActive)
        {
            SetIsActive(false);
            _gameManager.ConfirmCards();
            SfxManager.Instance.SetPlayerSfxVolume(false);
        }
    }

    public void CancelPressed()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && isActive)
        {
            //sound effect call
            SfxManager.Instance.PlaySFX(8885);
            SfxManager.Instance.PlaySFX(4295);

            _uiManager.DestroyConfirmCard();
            _gameManager.CancelCard();
            CancelCard.Invoke();
        }
    }
    #endregion

    #region Arrows
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
    #endregion

    #region Paused / Restart
    public void Paused()
    {
        _pauseMenu.TogglePause();
    }

    public void Restart()
    {
        _pauseMenu.RestartLevel();
    }
    #endregion
}