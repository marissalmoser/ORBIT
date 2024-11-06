/******************************************************************
 *    Author: Sky Turner 
 *    Contributors:
 *    Date Created: 10/13/24
 *    Description: Collectible Manager 
 *    
 *******************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleManager : MonoBehaviour
{
    #region singleton
    public static CollectibleManager Instance { get; private set; }
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
            _defaultList = collectibleStats.Select(item => item.Clone()).ToList();
        }
    }
    #endregion

    public List<CollectibleStats> collectibleStats;
    private List<CollectibleStats> _defaultList = new List<CollectibleStats>(); //copy list to hold default values

    public List<CollectibleStats> GetDefaultCollectibleList()
    {
        return _defaultList;
    }
    public List<CollectibleStats> GetEditedCollectibleList()
    {
        return collectibleStats;
    }


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
        for (int i = 0; i < 30; i++)
        {
            collectibleStats.Add(new CollectibleStats($"Level {i + 1}", i, true));
        }
    }

    /// <summary>
    /// Collects a collectible based on the build index of the scene
    /// </summary>
    public void CollectCollectible()
    {
        //update list if that level has a collectable
        int scene = SceneManager.GetActiveScene().buildIndex;
        if (collectibleStats[scene].HasCollectible())
        {
            collectibleStats[scene].CollectCollectible();
        }
    }

    public bool GetIsCollected()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        return collectibleStats[scene].GetIsCollected();
    }

    public bool HasCollectable()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        return collectibleStats[scene].HasCollectible();
    }
}
