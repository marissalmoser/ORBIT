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
    [SerializeField] private string _levelName;
    [SerializeField] private int _buildIndex;
    [SerializeField] private bool _isCollected;
    [SerializeField] private bool _hasCollectible;

    private CollectibleManager _collectibleManager;

    /// <summary>
    /// Initializes a new instance of the CollectibleStats class.
    /// </summary>
    /// <param name="levelName">The name of the level.</param>
    /// <param name="buildIndex">The build index of the level.</param>
    /// <param name="hasCollectible">Indicates if there is a collectible present in the level.</param>

    public CollectibleStats(string levelName, int buildIndex, bool hasCollectible)
    {
        _levelName = levelName;
        _buildIndex = buildIndex;
        _hasCollectible = hasCollectible;
        _isCollected = false;
    }

    /// <summary>
    /// Sets the isCollected stat to true
    /// </summary>
    /// <param name="index"></param>
    public void CollectCollectible()
    {
        _isCollected = true;
    }

    public bool GetIsCollected()
    {
        return _isCollected;
    }

    public bool HasCollectible()
    {
        return _hasCollectible;
    }
}
