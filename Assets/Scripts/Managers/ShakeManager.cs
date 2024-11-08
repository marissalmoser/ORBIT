/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 11/1/24
*    Description: This script handles the shakemanager
*******************************************************************/
using UnityEngine;
using Cinemachine;
using System.Collections;

public class ShakeManager : MonoBehaviour
{
    public static ShakeManager Instance;
    public CinemachineVirtualCamera virtualCamera;

    private CinemachineBasicMultiChannelPerlin _perlinNoise;
    private Coroutine _shakeCoroutine;

    #region singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    /// <summary>
    /// Static method for shaking the camera
    /// </summary>
    /// <param name="amplitude"></param>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    public static void ShakeCamera(float amplitude, float frequency, float duration)
    {
        if(ShakeSettings.isCameraShakeEnabled)
        {
            ShakeManager shakeManager = FindObjectOfType<ShakeManager>();
            if(shakeManager != null)
            {
                if(shakeManager._shakeCoroutine != null)
                {
                    shakeManager.StopCoroutine(shakeManager._shakeCoroutine);
                }
                shakeManager._shakeCoroutine = shakeManager.StartCoroutine(shakeManager.Shake(amplitude, frequency, duration));
            }
        }
    }

    /// <summary>
    /// Coroutine to Shake the camera
    /// </summary>
    /// <param name="amplitude"></param>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator Shake(float amplitude, float frequency, float duration)
    {
        // Set the noise parameters
        _perlinNoise.m_AmplitudeGain = amplitude;
        _perlinNoise.m_FrequencyGain = frequency;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset noise parameters
        _perlinNoise.m_AmplitudeGain = 0f;
        _perlinNoise.m_FrequencyGain = 0f;
    }

    /// <summary>
    /// Toggles shake on
    /// </summary>
    public void ToggleShakeOn()
    {
        ShakeSettings.isCameraShakeEnabled = true;
    }

    /// <summary>
    /// Toggles shake off
    /// </summary>
    public void ToggleShakeOff()
    {
        ShakeSettings.isCameraShakeEnabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShakeCamera(1, 1, 2);
        }
    }
}
