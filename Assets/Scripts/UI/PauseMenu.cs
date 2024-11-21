/******************************************************************
*    Author: Sky Turner
*    Contributors: Ryan Herwig
*    Date Created: November 21st 2024
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

        //Adds a listener to listen to when the sound effects slider value is changed
        _sfxSlider.onValueChanged.AddListener(v => 
        {
            if (v <= 0.01) // Gives a error tolerance to account for floating point errors
                SfxManager.Instance.SetSfxMixerVolume(0.01f); // Prevents Log(0) from occurring (which is undefined)
            else
                SfxManager.Instance.SetSfxMixerVolume(Mathf.Round(v * 100f) / 100f); // Removes floating point error calculations
        });

        //Adds a listener to listen to when the music slider value is changed
        _musicSlider.onValueChanged.AddListener(v => 
        {
            if (v <= 0.01) // Gives a error tolerance to account for floating point errors
                MusicManager.Instance.SetMusicMixerVolume(0.01f); // Prevents Log(0) from occurring (which is undefined)
            else
                MusicManager.Instance.SetMusicMixerVolume(Mathf.Round(v * 100f) / 100f); // Removes floating point error calculations                                                           
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
