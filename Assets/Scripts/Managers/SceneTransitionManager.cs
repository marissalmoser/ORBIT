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
    private Animator _anim;

    private int _currentSceneIndex;
    private int _sceneToLoad;

    private bool _animHasPlayed;
    private static bool _isloading;
    private bool _continue = true;

    [SerializeField] private GameObject _buttons;

    private void Start()
    {
        _cm = CollectibleManager.Instance;
        _currentSceneIndex = 0;
        _anim = GetComponentInChildren<Animator>();
    }

    #region public functions

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
        _sceneToLoad = sceneToLoad;
        _animHasPlayed = false;

        //determine which transition to play based on scene types
        CollectibleStats.SceneType currentType = _cm.collectibleStats[_currentSceneIndex].GetSceneType();
        CollectibleStats.SceneType nextType = _cm.collectibleStats[_sceneToLoad].GetSceneType();

        //level to level, should only be called on win (death uses ResetLevelOnDeath() )
        if(currentType == CollectibleStats.SceneType.Level && currentType == nextType)
        {
            StartCoroutine(LoadAndUnloadScene(true));      //TODO: change animation parameter to correct string
        }
        //menu to menu 
        else if(currentType == CollectibleStats.SceneType.Menu && currentType == nextType)
        {
            StartCoroutine(LoadAndUnloadScene(false));     //TODO: change animation parameter to correct string
        }
        //level to menu
        else if(currentType == CollectibleStats.SceneType.Level && nextType == CollectibleStats.SceneType.Menu)
        {
            StartCoroutine(LoadAndUnloadScene(false));     //TODO: change animation parameter to correct string
        }
        //menu to level
        else if (currentType == CollectibleStats.SceneType.Menu && nextType == CollectibleStats.SceneType.Level)
        {
            StartCoroutine(LoadAndUnloadScene(false));     //TODO: change animation parameter to correct string
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
    public void ResetLevel()
    {
        if(!_isloading)
        {
            StartCoroutine(ReloadScene());  //TODO: change animation parameter to correct string
        }
    }

    #endregion

    #region scene coroutines

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="animBoolName"></param>
    /// <returns></returns>
    private IEnumerator LoadAndUnloadScene(bool LoadButtons, string animBoolName = "TestSwipe")
    {
        //play animation
        _anim.SetBool(animBoolName, true);

        //wait until animation is completed
        while(!_animHasPlayed)
        {
            yield return null;
        }
        _animHasPlayed = false;

        //enable buttons
        if(LoadButtons)
        {
            _continue = false;
            _buttons.SetActive(true);
        }

        //start loading scene
        var nextScene = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);

        //wait for scene to finish loading
        while (!nextScene.isDone)
        {
            yield return null;
        }

        //wait for continue button if needed
        if(LoadButtons)
        {
            while(!_continue)
            {
                yield return null;
            }
        }

        _anim.SetBool(animBoolName, false);
        _isloading = false;
    }

    /// <summary>
    /// Used for player death and restating level
    /// </summary>
    /// <param name="animBoolName"></param>
    /// <returns></returns>
    private IEnumerator ReloadScene(string animBoolName = "TestSwipe")
    {
        print("reload");
        _isloading = true;
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        //play animation
        _anim.SetBool(animBoolName, true);

        //wait until animation is completed
        while (!_animHasPlayed)
        {
            yield return null;
        }
        _animHasPlayed = false;

        //start loading scene
        var nextScene = SceneManager.LoadSceneAsync(_currentSceneIndex, LoadSceneMode.Single);
        //nextScene.allowSceneActivation = false;

        //wait for scene to finish loading
        while (!nextScene.isDone)
        {
            yield return null;
        }

        //nextScene.allowSceneActivation = true;

        _anim.SetBool(animBoolName, false);
        _isloading = false;
    }

    //Used by the Replay and exit functions
    private IEnumerator ButtonLoadScene(int sceneToLoad)
    {
        //start loading scene
        var nextScene = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);

        //wait for scene to finish loading
        while (!nextScene.isDone)
        {
            yield return null;
        }

        //continue from buttons screen
        _continue = true;
    }

    #endregion

    #region button and anim event functions

    public void SetAnimHasPlayed()
    {
        _animHasPlayed = true;
    }

    /// <summary>
    /// continue buttons
    /// </summary>
    public void ContinueButtons()
    {
        _buttons.SetActive(false);
        _continue = true;
    }

    /// <summary>
    /// reset and replay level
    /// </summary>
    public void ReplayButtons()
    {
        StartCoroutine(ButtonLoadScene(_currentSceneIndex));
        _buttons.SetActive(false);
    }

    /// <summary>
    /// exit buttons screen to level select
    /// </summary>
    public void ExitButtons()
    {
        _buttons.SetActive(false);

        //determine which level select to load
        if(_sceneToLoad <= 8)       //1
        {
            StartCoroutine(ButtonLoadScene(2));
            return;
        }
        if (_sceneToLoad <= 15)     //2
        {
            StartCoroutine(ButtonLoadScene(9));
            return;
        }
        if (_sceneToLoad <= 22)     //3
        {
            StartCoroutine(ButtonLoadScene(16));
            return;
        }
        if (_sceneToLoad <= 28)     //4
        {
            StartCoroutine(ButtonLoadScene(23));
            return;
        }
        else                        //5
        {
            StartCoroutine(ButtonLoadScene(29));
            return;
        }

    }

    #endregion
}

///TODOs:
///canvas continue button for level -> level transitions
