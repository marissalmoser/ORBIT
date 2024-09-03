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
    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private Coroutine cameraMovementCoroutine;
    [SerializeField] private float cameraSpeedMult;

    private void Start()
    {
        // Subscribe to input events
        PlayerInput.currentActionMap["PanCamera"].started += ctx => PanCamera(ctx);
        PlayerInput.currentActionMap["PanCamera"].canceled += ctx => PanCameraCanceled(ctx);
    }

    private void PanCamera(InputAction.CallbackContext ctx)
    {
        // Read the input value from the context
        float panInput = ctx.ReadValue<float>();

        // Start or stop camera movement based on input
        if (cameraMovementCoroutine != null)
        {
            StopCoroutine(cameraMovementCoroutine);
        }
        cameraMovementCoroutine = StartCoroutine(MoveCamera(panInput));
    }

    private void PanCameraCanceled(InputAction.CallbackContext ctx)
    {
        // Stop camera movement when input is canceled
        if (cameraMovementCoroutine != null)
        {
            StopCoroutine(cameraMovementCoroutine);
        }
    }

    private IEnumerator MoveCamera(float direction)
    {
        CinemachineTrackedDolly dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        float movementSpeed = direction * cameraSpeedMult;

        while (Mathf.Abs(direction) > 0.01f)
        {
            dolly.m_PathPosition += movementSpeed * Time.deltaTime * -1;
            direction = PlayerInput.currentActionMap["PanCamera"].ReadValue<float>();
            yield return null;
        }
    }
}