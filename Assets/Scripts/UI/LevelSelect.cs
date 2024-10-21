/******************************************************************
*    Author: Sky Turner
*    Contributors: Elijah Vroman
*    Date Created: 9/5/24
*    Description: This script loads a scene for level selection
*******************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int sceneIndexOnWin;

    /// <summary>
    /// This function loads a scene based on the int passed through
    /// </summary>
    /// <param name="levelNumber">The build index for the scene</param>
    public void LoadLevel(int levelNumber)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(levelNumber);
    }


    /// <summary>
    /// Public method to return the scene index for gamemanager EV
    /// </summary>
    /// <returns></returns>
    public int GetSceneToGoOnWin()
    {
        return sceneIndexOnWin;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
