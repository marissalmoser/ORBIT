using UnityEngine;
using UnityEngine.InputSystem;

public class ConfirmationControls : MonoBehaviour
{

    private bool isConfirmationPressedDown;
    private bool mouseInConfirmationButton;

    private bool isCancelPressedDown;
    private bool mouseInCancelButton;

    private GameManager _gameManager;
    private UIManager _uiManager;

    public bool isActive;

    private Animator _anim;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        isConfirmationPressedDown = false;
        mouseInConfirmationButton = false;
        isCancelPressedDown = false;
        mouseInCancelButton = false;

        _anim = GetComponent<Animator>();
    }

    public void SetIsActive(bool isActive)
    {
        this.isActive = isActive;

        if(isActive)
        {
            _anim.SetTrigger("activeAnim");
            return;
        }

        _anim.SetTrigger("inactiveAnim");
    }

    //public void ConfirmationMouseEnter()
    //{
    //    mouseInConfirmationButton = true;
    //}

    //public void ConfirmationMouseExit()
    //{
    //    mouseInConfirmationButton = false;
    //}

    //public void ConfirmationPressed()
    //{
    //    if (Mouse.current.leftButton.wasPressedThisFrame)
    //    {
    //        isConfirmationPressedDown = true;
    //    }
    //}

    public void ConfirmationReleased()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && isActive)
        {
            //if (isConfirmationPressedDown && mouseInConfirmationButton && isActive)
            {
                SetIsActive(false);
                //isConfirmationPressedDown = false;
                _gameManager.ConfirmCards();
            }
        }
    }

    //public void CancelMouseEnter()
    //{
    //    mouseInCancelButton = true;
    //}

    //public void CancelMouseExit()
    //{
    //    mouseInCancelButton= false;
    //}

    //public void CancelPressed()
    //{
    //    if (Mouse.current.leftButton.wasPressedThisFrame)
    //    {
    //        isCancelPressedDown = true;
    //    }
    //}

    public void CancelReleased()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && isActive)
        {
            //if (isCancelPressedDown && mouseInCancelButton && isActive)
            {
                //sound effect call
                SfxManager.Instance.PlaySFX(8885);

                SetIsActive(false);
                //isCancelPressedDown = false;
                _uiManager.DestroyConfirmCard();
                _gameManager.CancelCard();
            }
        }
    }
}