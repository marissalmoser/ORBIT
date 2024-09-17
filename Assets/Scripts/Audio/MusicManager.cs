/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 9/12/24
 *    Description: Sfx manager singleton. Call to start a sound effect. Also includes 
 *    
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<Music> _music = new List<Music>();
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;
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
    }

    #region Setting ID in editor
    /// <summary>
    /// Whenever the list of sfx is edited, verifies all sfx have their own id
    /// </summary>
    private void OnValidate()
    {
        for (int i = _music.Count - 1; i >= 0 ; i--)
        {
            ValidateID(_music[i], i);
        }
    }

    /// <summary>
    /// Checks if the given index of _SFXs has a duplicate ID, if so calls GenerateID()
    /// to update it.
    /// </summary>
    /// <param name="sfx"></param>
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
    /// Plays the given sound effect. Finds the index of the specific sound effect,
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
    /// Stops the given sound effect from playing. Finds the index of the specific
    /// sound effect, and then stops the clip.
    /// </summary>
    /// <param name="name"></param>
    public void StopMusic(int id)
    {
        Music music = _music[_music.FindIndex(i => i.id == id)];
        music.source.Stop();
    }

    /// <summary>
    /// Fades in the given sound effect. Finds the index of the specific sound effect,
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
    /// Fades the given sound effect's volume to 0.
    /// </summary>
    /// <param name="id"></param>
    public void FadeOutMusic(int id)
    {
        Music music = _music[_music.FindIndex(i => i.id == id)];

        StartCoroutine(StartFade(music.source, 0, _fadeOutDuration));
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

    /// <summary>
    /// Temp for testing
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayMusic(7524);
        }
    }
}