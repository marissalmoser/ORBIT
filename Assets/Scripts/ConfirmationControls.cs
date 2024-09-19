using UnityEngine;

public class ConfirmationControls : MonoBehaviour
{

    private bool isConfirmationPressedDown;
    private bool mouseInConfirmationButton;

    private bool isCancelPressedDown;
    private bool mouseInCancelButton;

    private GameManager _gameManager;
    private UIManager _uiManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        isConfirmationPressedDown = false;
        mouseInConfirmationButton = false;
        isCancelPressedDown = false;
        mouseInCancelButton = false;
    }

    public void ConfirmationMouseEnter()
    {
        mouseInConfirmationButton = true;
    }

    public void ConfirmationMouseExit()
    {
        mouseInConfirmationButton = false;
    }

    public void ConfirmationPressed()
    {
        isConfirmationPressedDown = true;
    }

    public void ConfirmationReleased()
    {
        if (isConfirmationPressedDown && mouseInConfirmationButton)
        {
            isConfirmationPressedDown = false;
            _uiManager.DestroyConfirmCard();
            _gameManager.ConfirmCards();
        }
    }

    public void CancelMouseEnter()
    {
        mouseInCancelButton = true;
    }

    public void CancelMouseExit()
    {
        mouseInCancelButton= false;
    }

    public void CancelPressed()
    {
        isCancelPressedDown = true;
    }

    public void CancelReleased()
    {
        if (isCancelPressedDown && mouseInCancelButton)
        {
            isCancelPressedDown = false;
            _uiManager.DestroyConfirmCard();
            _gameManager.CancelCard();
        }
    }
}