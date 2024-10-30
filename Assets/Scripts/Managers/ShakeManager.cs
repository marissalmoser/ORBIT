using UnityEngine;
using Cinemachine;
using System.Collections;

public class ShakeManager : MonoBehaviour
{
    public static ShakeManager Instance;
    public CinemachineVirtualCamera virtualCamera;

    // Toggle for enabling/disabling camera shake
    public bool isCameraShakeEnabled = true;

    private CinemachineBasicMultiChannelPerlin perlinNoise;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
    }

    private IEnumerator Shake(float amplitude, float frequency, float duration)
    {
        // Set the noise parameters
        perlinNoise.m_AmplitudeGain = amplitude;
        perlinNoise.m_FrequencyGain = frequency;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset noise parameters
        perlinNoise.m_AmplitudeGain = 0f;
        perlinNoise.m_FrequencyGain = 0f;
    }

    public static void ToggleShake()
    {
        Instance.isCameraShakeEnabled = !Instance.isCameraShakeEnabled;
    }
}
