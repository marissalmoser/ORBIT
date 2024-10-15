/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 9/12/24
 *    Description: Sfx manager singleton. Call to start a sound effect.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SfxManager : MonoBehaviour
{
    [SerializeField] private List<SFX> _SFXs = new List<SFX>();
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;
    [SerializeField] private AudioMixer _masterMixer;

    private float _sfxCurrentVolume = 1; //volume set by player in options menu
    private float _playerSfxCurrentVolume; //volume set by player in oprions menu
    [SerializeField] private float _playerGhostVolume; //volume for the ghost player. Set to -80 to mute.
    private bool _ghostPlaying;

    public static SfxManager Instance { get; private set; }

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

        //create audio components and set fields
        for (int i = 0; i < _SFXs.Count; i++)
        {
            //need to be different game objects so sxfs can overlap and not cut off
            GameObject go = Instantiate(new GameObject(), transform);

            _SFXs[i].source = go.AddComponent<AudioSource>();
            _SFXs[i].source.outputAudioMixerGroup = _SFXs[i].mixer;
            _SFXs[i].source.volume = _SFXs[i].maxVolume;
            _SFXs[i].source.pitch = _SFXs[i].pitch;
            _SFXs[i].source.playOnAwake = false;
            _SFXs[i].source.loop = _SFXs[i].doLoop;
        }
    }

    #region Setting ID in editor
    /// <summary>
    /// Whenever the list of sfx is edited, verifies all sfx have their own id
    /// </summary>
    private void OnValidate()
    {
        for (int i = _SFXs.Count - 1; i >= 0 ; i--)
        {
            ValidateID(_SFXs[i], i);
        }
    }

    /// <summary>
    /// Checks if the given index of _SFXs has a duplicate ID, if so calls GenerateID()
    /// to update it.
    /// </summary>
    /// <param name="sfx"></param>
    private void ValidateID(SFX sfx, int index)
    {
        for (int i = _SFXs.Count - 1; i >= 0; i--)
        {
            //if i is the same as another 
            if (sfx.id == _SFXs[i].id && index != i)
            {
                GenerateID(sfx, index);
            }
        }
    }

    private void GenerateID(SFX sfx, int index)
    {
        sfx.id = UnityEngine.Random.Range(1000, 10000);
        ValidateID(sfx, index);
    }
    #endregion

    #region playing sfx functions
    /// <summary>
    /// Plays the given sound effect. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="name"></param>
    public void PlaySFX(int id)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.id == id)];
        sfx.source.clip = sfx.clips[UnityEngine.Random.Range(0, sfx.clips.Length)];
        sfx.source.volume = sfx.maxVolume;
        sfx.source.Play();
    }

    /// <summary>
    /// Stops the given sound effect from playing. Finds the index of the specific
    /// sound effect, and then stops the clip.
    /// </summary>
    /// <param name="name"></param>
    public void StopSFX(int id)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.id == id)];
        sfx.source.Stop();
    }

    /// <summary>
    /// Fades in the given sound effect. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="id"></param>
    public void FadeInSFX(int id)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.id == id)];
        sfx.source.clip = sfx.clips[UnityEngine.Random.Range(0, sfx.clips.Length)];
        sfx.source.volume = 0;
        sfx.source.Play();

        StartCoroutine(StartFade(sfx.source, sfx.maxVolume, _fadeInDuration));
    }

    /// <summary>
    /// Fades the given sound effect's volume to 0.
    /// </summary>
    /// <param name="id"></param>
    public void FadeOutSFX(int id)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.id == id)];

        StartCoroutine(StartFade(sfx.source, 0, _fadeOutDuration));
    }

    /// <summary>
    /// coroutine used by the fade in and fade out functions to fade a sfx to a
    /// specific volume.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="targetVolume"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator StartFade(AudioSource audioSource, float targetVolume, float duration)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if (targetVolume <= 0)
        {
            audioSource.Stop();
        }

        yield break;
    }

    #endregion

    #region mixer functions

    /// <summary>
    /// Sets the volume of the SFX mixer input 0 thru -80
    /// </summary>
    /// <param name="volume"></param>
    public void SetSfxMixerVolume(float volume)
    {
        _masterMixer.SetFloat("SfxVolume",  Mathf.Log(volume) * 20);
        _sfxCurrentVolume = volume;
        _playerSfxCurrentVolume = volume;
        print(volume);
        if (!_ghostPlaying)
        {
            _masterMixer.SetFloat("PlayerSfxVolume", Mathf.Log(volume) * 20);
        }
    }

    /// <summary>
    /// Sets the player sfx volume for the ghost state. True input will set the volume
    /// to the ghost's volume and false will set it back to normal.
    /// </summary>
    /// <param name="input"></param>
    public void SetPlayerSfxVolume(bool input)
    {
         if(input)
         {
            _masterMixer.SetFloat("PlayerSfxVolume", Mathf.Log(_playerGhostVolume) * 20);
            _ghostPlaying = true;
            return;
         }
        _masterMixer.SetFloat("PlayerSfxVolume", Mathf.Log(_playerSfxCurrentVolume) * 20);
        _ghostPlaying = false;
    }

    public float GetCurrentVolume()
    {
        return _sfxCurrentVolume;
    }

    #endregion

}