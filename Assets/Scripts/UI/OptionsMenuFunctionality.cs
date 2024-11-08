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
    [SerializeField] private Button _turnOnButton;
    [SerializeField] private Button _turnOffButton;

    /// <summary>
    /// Sets the sliders on the options menu to match the current volume.
    /// </summary>
    void Start()
    {
        _sfxSlider.value = SfxManager.Instance.GetCurrentVolume();
        _musicSlider.value = MusicManager.Instance.GetCurrentVolume();

        if (ShakeSettings.isCameraShakeEnabled)
        {
            _turnOffButton.gameObject.SetActive(false);
            _turnOnButton.gameObject.SetActive(true);
        }
        else
        {
            _turnOffButton.gameObject.SetActive(true);
            _turnOnButton.gameObject.SetActive(false);
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
