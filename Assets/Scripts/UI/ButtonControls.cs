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
    private ButtonsManager _buttonsManager;
    private PauseMenu _pauseMenu;

    public bool isActive;
    private Animator _anim;
    public static Action CancelCard;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        _buttonsManager = ButtonsManager.Instance;
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

            _gameManager.CancelCard();
            CancelCard.Invoke();
        }
    }

    public void AnyConfirmationButtonEnter()
    {
        ButtonsManager.Instance.currentCursor = _gameManager.currentCursor;
        _gameManager.SetCursor("Default");
    }

    public void AnyConfirmationButtonExit()
    {
        if (CardManager.Instance.lastConfirmationCard != null)
           _gameManager.SetCursor(ButtonsManager.Instance.currentCursor);
    }
    #endregion

    #region Arrows
    /// <summary>
    /// Increases the index of the card showing
    /// If the index is already at the highest value, set it to 0
    /// </summary>
    public void IncreaseIndex()
    {
        _buttonsManager.IncreaseIndex();
    }

    /// <summary>
    /// Decreases the index the card is showing
    /// If the index is already at the minimum value, set the index to the max value
    /// </summary>
    public void DecreaseIndex()
    {
        _buttonsManager.DecreaseIndex();
    }
    #endregion

    #region Paused / Restart
    public void Paused()
    {
        _pauseMenu.TogglePause();
    }

    public void Restart()
    {

        GameManager.Instance.SetCursor("Default");
        _pauseMenu.RestartLevel();
    }
    #endregion
}