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
