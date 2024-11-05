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
    /// This function loads a scene based on the int passed through. Checks if the
    /// the level is unlocked before loading it.
    /// </summary>
    /// <param name="levelNumber">The build index for the scene</param>
    public void LoadLevel(int levelNumber)
    {
        if (!CollectibleManager.Instance.collectibleStats[levelNumber].GetIsLocked())
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(levelNumber);
        }
    }

    /// <summary>
    /// Called in a level when it is won to go to the next level. Unlocks the next
    /// level and then loads it.
    /// </summary>
    /// <param name="levelNumber"></param>
    public void LoadLevelOnWin(int levelNumber)
    {
        CollectibleManager.Instance.SetActiveLevelCompleted();
        Time.timeScale = 1.0f;
        CollectibleManager.Instance.UnlockNextLevel(levelNumber);
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
