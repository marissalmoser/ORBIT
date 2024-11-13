/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 11/1/24
*    Description: This script contains the settings for the camera
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    public static CameraSettings Instance;

    public static bool isCameraShakeEnabled = true;
    public static float cameraSpeedMultiplier = 5f;

    #region singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }
    #endregion
}
