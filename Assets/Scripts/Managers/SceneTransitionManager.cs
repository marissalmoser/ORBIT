using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    #region Singleton
    [HideInInspector] public static SceneTransitionManager Instance;
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
    }
    #endregion

    private CollectibleManager _cm;
    private int _currentSceneIndex;
    private Animator _anim;

    private bool _animHasPlayed;
    private static bool _isloading;

    private void Start()
    {
        _cm = CollectibleManager.Instance;
        _currentSceneIndex = 0;
        _anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Call this function to load a scene with a scene transition.
    /// </summary>
    public void LoadNewScene(int sceneToLoad)
    {
        if(_isloading)
        {
            return;
        }

        _isloading = true;

        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _animHasPlayed = false;

        //determine which transition to play based on scene types
        CollectibleStats.SceneType currentType = _cm.collectibleStats[_currentSceneIndex].GetSceneType();
        CollectibleStats.SceneType nextType = _cm.collectibleStats[sceneToLoad].GetSceneType();

        //level to level
        if(currentType == CollectibleStats.SceneType.Level && currentType == nextType)
        {
            StartCoroutine(LoadAndUnloadScene(sceneToLoad));
        }
        //menu to menu 
        else if(currentType == CollectibleStats.SceneType.Menu && currentType == nextType)
        {
            StartCoroutine(LoadAndUnloadScene(sceneToLoad));      
        }
        //level to menu
        else if(currentType == CollectibleStats.SceneType.Level && nextType == CollectibleStats.SceneType.Menu)
        {
            StartCoroutine(LoadAndUnloadScene(sceneToLoad));
        }
        //menu to level
        else if (currentType == CollectibleStats.SceneType.Menu && nextType == CollectibleStats.SceneType.Level)
        {
            StartCoroutine(LoadAndUnloadScene(sceneToLoad));
        }
        //default
        else
        {
            print("Default Transition Error?");
        }

    }


    /// <summary>
    /// Call this function when the player dies, and the scene needs to be reset.
    /// </summary>
    public void ResetLevelOnDeath()
    {
        StartCoroutine(LoadAndUnloadScene(SceneManager.GetActiveScene().buildIndex));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="animBoolName"></param>
    /// <returns></returns>
    private IEnumerator LoadAndUnloadScene(int sceneToLoad = 0, string animBoolName = "TestSwipe")
    {
        //play animation
        _anim.SetBool(animBoolName, true);

        //wait until animation is completed
        while(!_animHasPlayed)
        {
            yield return null;
        }

        //disable current scene's event system
        GameObject es = GameObject.FindAnyObjectByType<EventSystem>().gameObject;
        es.SetActive(false);

        //start loading scene
        var nextScene = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        nextScene.allowSceneActivation = false;

        //wait for scene to activate
        while (!nextScene.isDone)
        {
            if (nextScene.progress <= 0.9f)
            {
                break;
            }
        }

        nextScene.allowSceneActivation = true;

        //wait for scene to finish loading
        while (!nextScene.isDone)
        {
            yield return null;
        }

        //disable active scene
        var unloadScene = SceneManager.UnloadSceneAsync(_currentSceneIndex);
        while(!unloadScene.isDone)
        {
            yield return null;
        }

        _anim.SetBool(animBoolName, false);
        _isloading = false;
    }

    public void SetAnimHasPlayed()
    {
        _animHasPlayed = true;
    }

}

///TODOs:
///canvas continue button for level -> level transitions
