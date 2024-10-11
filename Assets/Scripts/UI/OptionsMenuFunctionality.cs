using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuFunctionality : MonoBehaviour
{
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;

    void Start()
    {
        _sfxSlider.value = SfxManager.Instance.GetCurrentVolume();
        _musicSlider.value = MusicManager.Instance.GetCurrentVolume();
    }

    public void ActivateSFX()
    {
        SfxManager.Instance.PlaySFX(6893);
    }
}
