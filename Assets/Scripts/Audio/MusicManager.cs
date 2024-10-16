/******************************************************************
 *    Author: Sky Turner 
 *    Contributors: Marissa
 *    Date Created: 9/14/24
 *    Description: Music manager singleton
 *    
 *******************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static Action ChangeMusic;

    [SerializeField] private List<Music> _music = new List<Music>();

    [SerializeField] private AudioMixer _masterMixer;
    private float _currentMusicVolume = 1;

    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;

    #region Level Music IDs
    
    [Header("Level Music IDs")]
    
    [SerializeField] private int _mainMenuMusicID;
    [SerializeField] private int _levelSelectMusicID;
    [SerializeField] private int _levelMoveMusicID;
    [SerializeField] private int _levelTurnMusicID;
    [SerializeField] private int _levelJumpMusicID;
    [SerializeField] private int _levelClearMusicID;
    [SerializeField] private int _levelSwitchMusicID;
    [SerializeField] private int _level1MusicID;
    [SerializeField] private int _level2MusicID;
    [SerializeField] private int _level3MusicID;
    [SerializeField] private int _level4MusicID;
    #endregion

    public static MusicManager Instance { get; private set; }

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
        for (int i = 0; i < _music.Count; i++)
        {
            //need to be different game objects so sxfs can overlap and not cut off
            GameObject go = Instantiate(new GameObject(), transform);

            _music[i].source = go.AddComponent<AudioSource>();
            _music[i].source.outputAudioMixerGroup = _music[i].mixer;
            _music[i].source.volume = _music[i].maxVolume;
            _music[i].source.pitch = _music[i].pitch;
            _music[i].source.playOnAwake = false;
            _music[i].source.loop = _music[i].doLoop;
        }

        PlayLevelMusic();
    }

    #region Setting ID in editor
    /// <summary>
    /// Whenever the list of clips is edited, verifies all clips have their own id
    /// </summary>
    private void OnValidate()
    {
        for (int i = _music.Count - 1; i >= 0 ; i--)
        {
            ValidateID(_music[i], i);
        }
    }

    /// <summary>
    /// Checks if the given index of _music has a duplicate ID, if so calls GenerateID()
    /// to update it.
    /// </summary>
    /// <param name="music"></param>
    /// <param name="index"></param>
    private void ValidateID(Music music, int index)
    {
        for (int i = _music.Count - 1; i >= 0; i--)
        {
            //if i is the same as another 
            if (music.id == _music[i].id && index != i)
            {
                GenerateID(music, index);
            }
        }
    }

    private void GenerateID(Music music, int index)
    {
        music.id = UnityEngine.Random.Range(1000, 10000);
        ValidateID(music, index);
    }
    #endregion

    /// <summary>
    /// Plays the given music clip. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="name"></param>
    public void PlayMusic(int id)
    {
        Music music = _music[_music.FindIndex(i => i.id == id)];
        music.source.clip = music.clips[UnityEngine.Random.Range(0, music.clips.Length)];
        music.source.volume = music.maxVolume;
        music.source.Play();
    }

    /// <summary>
    /// Stops the given music clip from playing. Finds the index of the specific
    /// sound effect, and then stops the clip.
    /// </summary>
    /// <param name="id"></param>
    public void StopMusic(int id)
    {
        Music music = _music[_music.FindIndex(i => i.id == id)];
        music.source.Stop();
    }

    /// <summary>
    /// Fades in the given music clip. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="id"></param>
    public void FadeInMusic(int id)
    {
        Music music = _music[_music.FindIndex(i => i.id == id)];
        music.source.clip = music.clips[UnityEngine.Random.Range(0, music.clips.Length)];
        music.source.volume = 0;
        music.source.Play();

        StartCoroutine(StartFade(music.source, music.maxVolume, _fadeInDuration));
    }

    /// <summary>
    /// Fades the given music clip's volume to 0.
    /// </summary>
    /// <param name="id"></param>
    public void FadeOutMusic(int id)
    {
        Music music = _music[_music.FindIndex(i => i.id == id)];

        StartCoroutine(StartFade(music.source, 0, _fadeOutDuration));
    }

    /// <summary>
    /// Stops all currently playing music
    /// </summary>
    public void StopAllMusic()
    {
        foreach(var music in _music)
        {
            if(music.source.isPlaying)
            {
                music.source.Stop();
            }
        }
    }

    /// <summary>
    /// coroutine used by the fade in and fade out functions to fade a clip to a
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

    private void OnEnable()
    {
        MusicManager.ChangeMusic += OnChangeMusic;
    }

    private void OnDisable()
    {
        MusicManager.ChangeMusic -= OnChangeMusic;
    }

    /// <summary>
    /// Plays the levels music for the scene when it is loaded
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnChangeMusic()
    {
        PlayLevelMusic();
    }

    /// <summary>
    /// Uses the serialized fields for level music IDs to play the song based
    /// on what level is loaded.
    /// </summary>
    private void PlayLevelMusic()
    {
        Scene scene = SceneManager.GetActiveScene();
        
        StopAllMusic();

        if (scene.buildIndex == 0) // Level Select
        {
            FadeInMusic(_levelSelectMusicID);
        }
        else if (scene.buildIndex == 1) // Move Level
        {
            FadeInMusic(_levelMoveMusicID);
        }
        else if (scene.buildIndex == 2) // Turn Level
        {
            FadeInMusic(_levelTurnMusicID);
        }
        else if (scene.buildIndex == 3) // Jump Level
        {
            FadeInMusic(_levelJumpMusicID);
        }
        else if (scene.buildIndex == 4) // Clear Level
        {
            FadeInMusic(_levelClearMusicID);
        }
        else if (scene.buildIndex == 5) // Switch Level
        {
            FadeInMusic(_levelSwitchMusicID);
        }
        else if (scene.buildIndex == 6) 
        {
            FadeInMusic(_level1MusicID);
        }
        else if (scene.buildIndex == 7)
        {
            FadeInMusic(_level2MusicID);
        }
        else if (scene.buildIndex == 8)
        {
            FadeInMusic(_level3MusicID);
        }
        else if (scene.buildIndex == 9)
        {
            FadeInMusic(_level4MusicID);
        }
    }

    #region mixer functions

    /// <summary>
    /// Sets the volume of the music mixer input 0 thru -80
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicMixerVolume(float volume)
    {
        _masterMixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
        _currentMusicVolume = volume;
    }

    public float GetCurrentVolume()
    {
        return _currentMusicVolume;
    }

    #endregion
}