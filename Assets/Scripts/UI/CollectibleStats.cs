/******************************************************************
 *    Author: Sky Turner 
 *    Contributors: Marissa, Elijah
 *    Date Created: 10/14/24
 *    Description: Collectible Stats Class
 *    
 *******************************************************************/
using System;
using UnityEngine;

[Serializable]
public class CollectibleStats
{
    public enum SceneType
    {
        None, Menu, Level, Screen
    }
    [SerializeField] private string _levelName;
    [SerializeField] private int _buildIndex;
    [SerializeField] private bool _isCollected;
    [SerializeField] private bool _hasCollectible;
    [SerializeField] private bool _IsLocked;
    [SerializeField] private SceneType _sceneType;

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
    /// A method for LINQ to access and make a deep copy of our custom class
    /// </summary>
    /// <returns></returns>
    public CollectibleStats Clone()
    {
        return new CollectibleStats(_levelName, _buildIndex, _hasCollectible)
        {
            _isCollected = this._isCollected
        };
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
    public SceneType GetSceneType()
    {
        return _sceneType;
    }
    public bool GetIsLocked()
    {
        return _IsLocked;
    }
    public void SetIsLocked(bool isLocked)
    {
        _IsLocked = isLocked;
    }
    public void SetIsCollected(bool status)
    {
        _isCollected = status;
    }
    public void SetSceneType(SceneType type)
    {
        _sceneType = type;
    }
    public string GetLevelName()
    {
        return _levelName;
    }
}
