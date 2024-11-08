/******************************************************************
 *    Author: Sky Turner 
 *    Contributors:
 *    Date Created: 10/10/24
 *    Description: Contains functionality for the options menu.
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

    private void Awake()
    {
        // Singleton pattern
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

    public void TurnOnShake()
    {
        ShakeSettings.isCameraShakeEnabled = true;
        Debug.Log("Shake Turned On");
    }

    public void TurnOffShake()
    {
        ShakeSettings.isCameraShakeEnabled = false;
        Debug.Log("Shake Turned Off");
    }

    public bool GetCurrentToggle()
    {
        return ShakeSettings.isCameraShakeEnabled;
    }
}
