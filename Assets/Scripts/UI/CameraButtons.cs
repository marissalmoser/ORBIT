using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraButtons : MonoBehaviour
{
    public static Action<float> CameraButtonPressed;
    public static Action CameraButtonReleased;

    /// <summary>
    /// Tells the camera script that the button was pressed
    /// </summary>
    /// <param name="dir"></param>
    public void ButtonPressed(float dir)
    {
        CameraButtonPressed?.Invoke(dir);
    }

    /// <summary>
    /// Tells the camera button that the button was released
    /// </summary>
    public void ButtonCanceled()
    {
        CameraButtonReleased?.Invoke();
    }
}
