/******************************************************************
 *    Author: Sky Turner 
 *    Contributors: Marissa
 *    Date Created: 9/13/24
 *    Description: Collectible Manager 
 *    
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

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

    public List<CollectibleStats> collectableStats;

    private CollectibleManager()
    {
        collectableStats = new List<CollectibleStats>(30);
        InitializeCollectibles();
    }

    /// <summary>
    /// Initializes collectibleStats list
    /// </summary>
    private void InitializeCollectibles()
    {
        for(int i = 0; i < 30; i++)
        {
            collectableStats.Add(new CollectibleStats($"Level {i + 1}", i, true));
        }
    }
}
