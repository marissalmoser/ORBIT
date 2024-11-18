/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 9/11/24
*    Description: This script handles the pause menu
*******************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;
    private GameObject _pauseMenu;
    private string _tempCurrentCursor;

    private float _sfxVolume;
    private float _musicVolume;

    private void Start()
    {
        // Subscribe to input events
        _playerInput.currentActionMap["Pause"].performed += ctx => TogglePause();
        _pauseMenu = transform.Find("PauseMenu").gameObject;
        _tempCurrentCursor = "";

        _sfxSlider.onValueChanged.AddListener(v => 
        {
            if (v <= 0.01)
                SfxManager.Instance.SetSfxMixerVolume(0.01f); // Prevents Log(0) from occurring (which is undefined)
            else
                SfxManager.Instance.SetSfxMixerVolume(Mathf.Round(v * 100f) / 100f);
        });
        _musicSlider.onValueChanged.AddListener(v => 
        {
            if (v <= 0.01)
                MusicManager.Instance.SetMusicMixerVolume(0.01f);
            else
                MusicManager.Instance.SetMusicMixerVolume(Mathf.Round(v * 100f) / 100f);
        });
    }

    /// <summary>
    /// Handles toggling the pause menu
    /// </summary>
    public void TogglePause()
    {
        if (_pauseMenu != null && !_pauseMenu.activeInHierarchy)
        {
            _pauseMenu.SetActive(true);
            _tempCurrentCursor = ButtonsManager.Instance.currentCursor;
            ButtonsManager.Instance.currentCursor = "Default";
            GameManager.Instance.SetCursor("Default");
            Time.timeScale = 0f;
        }
        else if (_pauseMenu != null && _pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 1f;
            _pauseMenu.SetActive(false);
            GameManager.Instance.SetCursor(_tempCurrentCursor);
        }
    }

    /// <summary>
    /// Restarts the level
    /// </summary>
    public void RestartLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        ButtonsManager.Instance.currentCursor = "Default";
        GameManager.Instance.SetCursor("Default");
        SceneTransitionManager.Instance.ResetLevel();
    }

    /// <summary>
    /// This function loads a scene based on the int passed through
    /// </summary>
    /// <param name="levelNumber">The build index for the scene</param>
    public void LoadLevel(int levelNumber)
    {
        Time.timeScale = 1.0f;
        //SceneManager.LoadScene(levelNumber);
        SceneTransitionManager.Instance.LoadNewScene(levelNumber);
    }
}
