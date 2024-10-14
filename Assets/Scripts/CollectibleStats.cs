/******************************************************************
 *    Author: Sky Turner 
 *    Contributors: Marissa
 *    Date Created: 10/14/24
 *    Description: Collectible Stats Class
 *    
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleStats
{
    public string LevelName { get; set; }
    public int BuildIndex { get; set; }
    public bool IsCollected { get; private set; }
    public bool HasCollectible { get; set; }

    /// <summary>
    /// Contains the necessary information for collectibles
    /// </summary>
    /// <param name="levelName"></param>
    /// <param name="buildIndex"></param>
    /// <param name="hasCollectible"></param>
    public CollectibleStats(string levelName, int buildIndex, bool hasCollectible)
    {
        LevelName = levelName;
        BuildIndex = buildIndex;
        HasCollectible = hasCollectible;
        IsCollected = false;
    }

    /// <summary>
    /// Sets the collectible to collected
    /// </summary>
    /// <param name="collected"></param>
    public void SetIsCollected(bool collected)
    {
        IsCollected = collected;
    }
}
