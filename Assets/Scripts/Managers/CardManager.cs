using UnityEngine;
using UnityEngine.InputSystem;

public class CardManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    private InputAction clickAction;

    private Vector2 mousePosition;
    private bool isMousePressedDown;
    void Start()
    {
        clickAction = playerInput.currentActionMap.FindAction("Click");

        clickAction.started += Click_Started;
        clickAction.canceled += Click_Canceled;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If the click has been pressed down but not released
        if (isMousePressedDown)
        {
            mousePosition = Mouse.current.position.ReadValue();
            print(mousePosition);
        }
    }

    private void Click_Started(InputAction.CallbackContext obj)
    {
        //Mouse is pressed down
        isMousePressedDown = true;
    }
    private void Click_Canceled(InputAction.CallbackContext obj)
    {
        //Mouse has been released
        isMousePressedDown = false;
    }
}
