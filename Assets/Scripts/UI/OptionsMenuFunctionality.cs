/******************************************************************
 *    Author: Marissa 
 *    Contributors: Sky Turner
 *    Date Created: 10/10/24
 *    Description: Contains functionality for the options menu.
 *******************************************************************/
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuFunctionality : MonoBehaviour
{
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _cameraSpeedSlider;
    [SerializeField] private Button _turnOffButton;
    [SerializeField] private Button _turnOnButton;

    /// <summary>
    /// Sets the sliders on the options menu to match the current volume.
    /// </summary>
    void Start()
    {
        _sfxSlider.value = SfxManager.Instance.GetCurrentVolume();
        _musicSlider.value = MusicManager.Instance.GetCurrentVolume();
        _cameraSpeedSlider.value = CameraController.Instance.GetCurrentCameraSpeed();

        _cameraSpeedSlider.onValueChanged.AddListener(delegate { CameraController.Instance.SetCameraSpeed(_cameraSpeedSlider.value); });

        if(CameraSettings.isCameraShakeEnabled)
        {
            _turnOnButton.gameObject.SetActive(true);
            _turnOffButton.gameObject.SetActive(false);
        }
        else
        {
            _turnOnButton.gameObject.SetActive(false);
            _turnOffButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Activates the button SFX.
    /// </summary>
    public void ActivateSFX()
    {
        SfxManager.Instance.PlaySFX(6893);
    }
}