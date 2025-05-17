/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 9/2/24
*    Description: This script handles the camera movement
*******************************************************************/

using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private Coroutine _cameraMovementCoroutine;

    public static CameraController Instance;

    private bool _isDragging = false;
    private bool _isPanning = false;
    private bool _isButtoning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        // Store the action map reference
        var actionMap = _playerInput.currentActionMap;

        // Subscribe to input events
        actionMap["PanCamera"].started += PanCamera;
        actionMap["PanCamera"].canceled += PanCameraCanceled;

        actionMap["DragCamera"].performed += DragCamera;
        actionMap["DragCamera"].canceled += DragCameraCanceled;

        CameraButtons.CameraButtonReleased += PanCameraCanceledButton;
        CameraButtons.CameraButtonPressed += PanCameraButton;
    }

    private void OnDisable()
    {
        var actionMap = _playerInput.currentActionMap;

        // Unsubscribe from input events
        actionMap["PanCamera"].started -= PanCamera;
        actionMap["PanCamera"].canceled -= PanCameraCanceled;

        actionMap["DragCamera"].performed -= DragCamera;
        actionMap["DragCamera"].canceled -= DragCameraCanceled;

        CameraButtons.CameraButtonReleased -= PanCameraCanceledButton;
        CameraButtons.CameraButtonPressed -= PanCameraButton;
    }

    /// <summary>
    /// Initiates camera panning based on player input.
    /// </summary>
    private void PanCamera(InputAction.CallbackContext ctx)
    {
        if (_isDragging) return;
        _isPanning = true;

        // Start moving the camera with the current input
        float panInput = ctx.ReadValue<float>();

        // Stop any ongoing camera movement
        if (_cameraMovementCoroutine != null)
        {
            StopCoroutine(_cameraMovementCoroutine);
        }

        _cameraMovementCoroutine = StartCoroutine(MoveCamera(panInput));
    }

    /// <summary>
    /// Stops camera panning when input is canceled.
    /// </summary>
    private void PanCameraCanceled(InputAction.CallbackContext ctx)
    {
        // Stop camera movement when input is canceled
        if (_cameraMovementCoroutine != null)
        {
            StopCoroutine(_cameraMovementCoroutine);
            _cameraMovementCoroutine = null; // Clear reference
        }
        _isPanning = false;
    }


    /// <summary>
    /// Initiates camera panning based on button.
    /// </summary>
    private void PanCameraButton(float panInput)
    {
        if (_isDragging) return;
        _isButtoning = true;

        // Stop any ongoing camera movement
        if (_cameraMovementCoroutine != null)
        {
            StopCoroutine(_cameraMovementCoroutine);
        }

        _cameraMovementCoroutine = StartCoroutine(MoveCameraUpdate(panInput));
    }

    /// <summary>
    /// Stops camera panning when button is canceled.
    /// </summary>
    private void PanCameraCanceledButton()
    {
        // Stop camera movement when input is canceled
        if (_cameraMovementCoroutine != null)
        {
            StopCoroutine(_cameraMovementCoroutine);
            _cameraMovementCoroutine = null; // Clear reference
        }
        _isButtoning = false;
    }

    /// <summary>
    /// Moves the camera along the Cinemachine Dolly path based on direction for buttons.
    /// </summary>
    /// <param name="direction">Direction input for camera movement.</param>
    private IEnumerator MoveCameraUpdate(float direction)
    {
        CinemachineTrackedDolly dolly = _virtualCamera.GetCinemachineComponent
            <CinemachineTrackedDolly>();
        float movementSpeed = direction * CameraSettings.cameraSpeedMultiplier;

        while (_isButtoning)
        {
            dolly.m_PathPosition += movementSpeed * Time.deltaTime * -1;
            direction = _playerInput.currentActionMap["PanCamera"].ReadValue<float>();
            yield return null;
        }
        _isButtoning = false;
        _cameraMovementCoroutine = null; // Clear reference when done
    }

    /// <summary>
    /// Moves the camera along the Cinemachine Dolly path based on direction.
    /// </summary>
    /// <param name="direction">Direction input for camera movement.</param>
    private IEnumerator MoveCamera(float direction)
    {
        CinemachineTrackedDolly dolly = _virtualCamera.GetCinemachineComponent
            <CinemachineTrackedDolly>();
        float movementSpeed = direction * CameraSettings.cameraSpeedMultiplier;

        while (Mathf.Abs(direction) > 0.01f)
        {
            dolly.m_PathPosition += movementSpeed * Time.deltaTime * -1;
            direction = _playerInput.currentActionMap["PanCamera"].ReadValue<float>();
            yield return null;
        }
        _cameraMovementCoroutine = null; // Clear reference when done
    }

    /// <summary>
    /// Initiates camera movement based on mouse input
    /// </summary>
    /// <param name="ctx"></param>
    private void DragCamera(InputAction.CallbackContext ctx)
    {
        if (_isPanning) return;
        _isDragging = true;
        Vector2 mouseDelta = ctx.ReadValue<Vector2>();
        MoveCameraWithMouse(mouseDelta.x);
    }

    private void DragCameraCanceled(InputAction.CallbackContext ctx)
    {
        _isDragging = false;
    }

    /// <summary>
    /// Moves the camera along the cinemachine dolly
    /// </summary>
    /// <param name="deltaX"></param>
    private void MoveCameraWithMouse(float deltaX)
    {
        if (_virtualCamera == null) return;

        float direction = 1;
        if (deltaX < 0)
            direction = -1;

        CinemachineTrackedDolly dolly = _virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        //print(direction * CameraSettings.cameraSpeedMultiplier * Time.deltaTime * -1);
        dolly.m_PathPosition += direction * CameraSettings.cameraSpeedMultiplier * Time.deltaTime * -1;
    }

    public float GetCurrentCameraSpeed()
    {
        return CameraSettings.cameraSpeedMultiplier;
    }

    public void SetCameraSpeed(float speed)
    {
        CameraSettings.cameraSpeedMultiplier = speed;
    }
}
