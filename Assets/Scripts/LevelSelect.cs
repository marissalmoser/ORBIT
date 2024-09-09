/******************************************************************
*    Author: Sky Turner
*    Contributors: 
*    Date Created: 9/5/24
*    Description: This script loads a scene for level selection
*******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    /// <summary>
    /// This function loads a scene based on the int passed through
    /// </summary>
    /// <param name="levelNumber">The build index for the scene</param>
   public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }
}
