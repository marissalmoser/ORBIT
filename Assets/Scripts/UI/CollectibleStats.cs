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
    [SerializeField] private bool _IsCompleted;
    [SerializeField] private SceneType _sceneType;

    private CollectibleManager _collectibleManager;


    /// <summary>
    /// A method for LINQ to access and make a deep copy of our custom class
    /// </summary>
    /// <returns></returns>
    public CollectibleStats Clone()
    {
        return new CollectibleStats()
        {
            _isCollected = this._isCollected,
            _sceneType = this._sceneType,
            _IsLocked = this._IsLocked,
            _IsCompleted = this._IsCompleted,
            _levelName = this._levelName,
            _buildIndex = this._buildIndex,
            _hasCollectible = this._hasCollectible
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

    public bool GetIsCompleted()
    {
        return _IsCompleted;
    }

    public void SetIsCompleted(bool input)
    {
        _IsCompleted = input;
    }
}
