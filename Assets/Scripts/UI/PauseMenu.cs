/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 9/11/24
*    Description: This script handles the pause menu
*******************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private GameObject _pauseMenu;
    private string _tempCurrentCursor;

    private void Start()
    {
        // Subscribe to input events
        _playerInput.currentActionMap["Pause"].performed += ctx => TogglePause();
        _pauseMenu = transform.Find("PauseMenu").gameObject;
        _tempCurrentCursor = "";
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

    public void SetSfxVolume(float volume)
    {
        SfxManager.Instance.SetSfxMixerVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        MusicManager.Instance.SetMusicMixerVolume(volume);
    }
}
