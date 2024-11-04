using UnityEngine;
using Cinemachine;
using System.Collections;

public class ShakeManager : MonoBehaviour
{
    public static ShakeManager Instance;
    public CinemachineVirtualCamera virtualCamera;

    // Toggle for enabling/disabling camera shake
    public bool isCameraShakeEnabled = true;

    private CinemachineBasicMultiChannelPerlin _perlinNoise;

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

    // Static method for shaking the camera
    public static void ShakeCamera(float amplitude, float frequency, float duration)
    {
        if (Instance != null && Instance.isCameraShakeEnabled)
        {
            Instance.StartCoroutine(Instance.Shake(amplitude, frequency, duration));
        }
        if(Instance != null && !Instance.isCameraShakeEnabled)
        {
            Instance.StartCoroutine(Instance.Shake(0, 0, 0));
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

    public static void ToggleShake()
    {
        Instance.isCameraShakeEnabled = !Instance.isCameraShakeEnabled;

        if(!Instance.isCameraShakeEnabled)
        {
            ShakeCamera(0, 0, 0); // disables shake if currently shaking
        }
    }

    public bool GetCurrentToggle()
    {
        return isCameraShakeEnabled;
    }
}
