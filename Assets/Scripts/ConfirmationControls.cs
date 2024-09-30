using UnityEngine;
using UnityEngine.InputSystem;

public class ConfirmationControls : MonoBehaviour
{
    private GameManager _gameManager;
    private UIManager _uiManager;

    public bool isActive;

    private Animator _anim;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

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
            print("RETURN");
            return;
        }

        //sets button to input state
        isActive = input;

        if(input)
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
            _gameManager.ConfirmCards();
        }
    }

    public void CancelPressed()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && isActive)
        {
            //sound effect call
            SfxManager.Instance.PlaySFX(8885);

            _uiManager.DestroyConfirmCard();
            _gameManager.CancelCard();
        }
    }
}