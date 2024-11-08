/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 11/1/24
*    Description: This script contains the settings for the shaking camera
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeSettings : MonoBehaviour
{
    public static ShakeSettings Instance;

    public static bool isCameraShakeEnabled = true;

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
