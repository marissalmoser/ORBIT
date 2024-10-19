/******************************************************************
 *    Author: Sky Turner 
 *    Contributors:
 *    Date Created: 10/13/24
 *    Description: Collectible Manager 
 *    
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }
    private CollectibleStats _collectibleStats;

    #region singleton
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

    public List<CollectibleStats> collectibleStats;

    /// <summary>
    /// Initializes a new instance of the CollectibleManager class.
    /// Sets up the collectibleStats list with 30 collectible items.
    /// </summary>
    private CollectibleManager()
    {
        collectibleStats = new List<CollectibleStats>(30);
        InitializeCollectibles();
    }

    /// <summary>
    /// Initializes the collectibleStats list with 30 collectible items.
    /// Each collectible corresponds to a level.
    /// </summary>
    private void InitializeCollectibles()
    {
        for(int i = 0; i < 30; i++)
        {
            collectibleStats.Add(new CollectibleStats($"Level {i + 1}", i, true));
        }
    }

    /// <summary>
    /// Collects a collectible based on the build index of the scene
    /// </summary>
    public void CollectCollectible()
    {
        //update list
        int scene = SceneManager.GetActiveScene().buildIndex;
        collectibleStats[scene].CollectCollectible();
        print(scene);
    }

    public bool GetIsCollected()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        return collectibleStats[scene].GetIsCollected();
    }
}
