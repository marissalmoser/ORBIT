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
using Unity.VisualScripting;
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

    private int _currentWorld = 0;
    private int _newWorld = 0;

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

        FadeInMusic(0);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnChangeMusic;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnChangeMusic;
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

    #region Playing/Fading/Stopping music
    /// <summary>
    /// Plays the given music clip. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="name"></param>
    public void PlayMusic(int id)
    {
        Music music = _music[id];
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
        Music music = _music[id];
        music.source.Stop();
    }

    /// <summary>
    /// Fades in the given music clip. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="id"></param>
    public void FadeInMusic(int id)
    {
        Music music = _music[id];
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
                FadeOutMusic(music.id);
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
    #endregion

    #region Changing music
    /// <summary>
    /// Invoked when a new scene is loaded, checks which music should be playing
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnChangeMusic(Scene scene, LoadSceneMode mode)
    {
        PlayLevelMusic(GetWorld(scene.buildIndex));
    }

    /// <summary>
    /// Gets which world should be playing based on the build index.
    /// </summary>
    /// <returns></returns>
    private int GetWorld(int scene)
    {
        switch (scene)
        {
            case 0:
            case 1:
            case 2:
                _newWorld = 0;
                break;
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                _newWorld = 1;
                break;
            case 9:
                _newWorld = 0;
                break;
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
                _newWorld = 2;
                break;
            case 16:
                _newWorld = 0;
                break;
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
                _newWorld = 3;
                break;
            case 23:
                _newWorld = 0;
                break;
            case 24:
            case 25:
            case 26:
            case 27:
            case 28:
                _newWorld = 4;
                break;
            case 29:
                _newWorld = 0;
                break;
            case 30:
            case 31:
            case 32:
            case 33:
                _newWorld = 5;
                break;
            case 34:
            default:
                _newWorld = 0;
                break;
        }
        return _newWorld;
    }

    /// <summary>
    /// Uses the serialized fields for level music IDs to play the song based
    /// on what level is loaded.
    /// </summary>
    private void PlayLevelMusic(int newWorld)
    {
        //if the player is in the same world, don't change the music.
        if(_newWorld == _currentWorld)
        {
            return;
        }

        //play new music track
        StopAllMusic();
        FadeInMusic(_music[newWorld].id);
        _currentWorld = newWorld;
    }
    #endregion

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