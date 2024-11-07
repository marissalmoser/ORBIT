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

    private void Start()
    {
        // Subscribe to input events
        _playerInput.currentActionMap["Pause"].performed += ctx => TogglePause();
        _pauseMenu = transform.Find("PauseMenu").gameObject;
    }
    
    /// <summary>
    /// Handles toggling the pause menu
    /// </summary>
    public void TogglePause()
    {
        if (_pauseMenu != null && !_pauseMenu.activeInHierarchy)
        {
            _pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (_pauseMenu != null && _pauseMenu.activeInHierarchy)
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
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
