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
    [SerializeField] private float _cameraSpeedMult;

    private void Start()
    {
        // Subscribe to input events
        _playerInput.currentActionMap["PanCamera"].started += ctx => 
        PanCamera(ctx);

        _playerInput.currentActionMap["PanCamera"].canceled += ctx => 
        PanCameraCanceled(ctx);
    }

    private void OnDisable()
    {
        _playerInput.currentActionMap["PanCamera"].started -= ctx =>
        PanCamera(ctx);

        _playerInput.currentActionMap["PanCamera"].canceled -= ctx =>
        PanCameraCanceled(ctx);
    }

    /// <summary>
    /// This function handles when the PanCamera action is started
    /// </summary>
    /// <param name="ctx">Input from player through arrow keys</param>
    private void PanCamera(InputAction.CallbackContext ctx)
    {
        // Read the input value from the context
        float panInput = ctx.ReadValue<float>();

        // Start or stop camera movement based on input
        if (_cameraMovementCoroutine != null)
        {
            StopCoroutine(_cameraMovementCoroutine);
        }
        _cameraMovementCoroutine = StartCoroutine(MoveCamera(panInput));
    }

    /// <summary>
    /// This function handles when the PanCamera action is canceled
    /// </summary>
    /// <param name="ctx">Input from player through arrow keys</param>
    private void PanCameraCanceled(InputAction.CallbackContext ctx)
    {
        // Stop camera movement when input is canceled
        if (_cameraMovementCoroutine != null)
        {
            StopCoroutine(_cameraMovementCoroutine);
        }
    }

    /// <summary>
    /// This function actually moves the camera on the Cinemachine Dolly
    /// </summary>
    /// <param name="direction">Which direction the camera is moving on the
    /// dolly</param>
    /// <returns></returns>
    private IEnumerator MoveCamera(float direction)
    {
        CinemachineTrackedDolly dolly = _virtualCamera.GetCinemachineComponent
            <CinemachineTrackedDolly>();
        float movementSpeed = direction * _cameraSpeedMult;

        while (Mathf.Abs(direction) > 0.01f)
        {
            dolly.m_PathPosition += movementSpeed * Time.deltaTime * -1;
            direction = _playerInput.currentActionMap["PanCamera"].ReadValue<float>();
            yield return null;
        }
    }
}