using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _pauseMenu;
    private bool _isPaused = false;

    private void Start()
    {
        // Subscribe to input events
        _playerInput.currentActionMap["Pause"].performed += ctx =>
            TogglePause();
    }
    
    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            _pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            _pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
