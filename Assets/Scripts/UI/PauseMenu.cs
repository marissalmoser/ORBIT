/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 9/11/24
*    Description: This script handles the pause menu
*******************************************************************/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _pauseMenu;
    private bool _isPaused = false;

    private void Start()
    {
        // Subscribe to input events
        _playerInput.currentActionMap["Pause"].performed += ctx => TogglePause();
    }
    
    /// <summary>
    /// Handles toggling the pause menu
    /// </summary>
    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused && _pauseMenu != null)
        {
            _pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else if(!_isPaused && _pauseMenu != null)
        {
            _pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Restarts the level
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    /// <summary>
    /// This function loads a scene based on the int passed through
    /// </summary>
    /// <param name="levelNumber">The build index for the scene</param>
    public void LoadLevel(int levelNumber)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(levelNumber);
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
