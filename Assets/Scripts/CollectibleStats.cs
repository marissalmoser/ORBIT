/******************************************************************
 *    Author: Sky Turner 
 *    Contributors: Marissa
 *    Date Created: 10/14/24
 *    Description: Collectible Stats Class
 *    
 *******************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CollectibleStats
{
    [SerializeField] string LevelName;
    [SerializeField] int BuildIndex;
    [SerializeField] bool IsCollected;
    [SerializeField] bool HasCollectible;

    /// <summary>
    /// Initializes a new instance of the CollectibleStats class.
    /// </summary>
    /// <param name="levelName">The name of the level.</param>
    /// <param name="buildIndex">The build index of the level.</param>
    /// <param name="hasCollectible">Indicates if there is a collectible present in the level.</param>

    public CollectibleStats(string levelName, int buildIndex, bool hasCollectible)
    {
        LevelName = levelName;
        BuildIndex = buildIndex;
        HasCollectible = hasCollectible;
        IsCollected = false;
    }

    /// <summary>
    /// Sets the collectible state to collected or not.
    /// </summary>
    /// <param name="collected">True if the collectible is collected, false otherwise.</param>
    public void SetIsCollected(bool collected)
    {
        IsCollected = collected;
    }
}
