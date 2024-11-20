/******************************************************************
 *    Author: Sky Turner 
 *    Contributors: Marissa
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

    /// <summary>
    /// Called when a level is won, if the next scene is a level and not a challenge
    /// level, unlocks it
    /// </summary>
    /// <param name="LevelToUnlock"></param>
    public void UnlockNextLevel(int LevelToUnlock)
    {
        //if the next scene is a level and not a challenge level, unlock it
        if(collectibleStats[LevelToUnlock].GetSceneType() == CollectibleStats.SceneType.Level &&
            (LevelToUnlock != 8 || LevelToUnlock != 15 || LevelToUnlock != 22 || LevelToUnlock != 28 || LevelToUnlock != 33))
        {
            collectibleStats[LevelToUnlock].SetIsLocked(false);
            return;
        }
    }

    /// <summary>
    /// Gets the active scene and sets it to completed.
    /// </summary>
    public void SetActiveLevelCompleted()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        collectibleStats[scene].SetIsCompleted(true);
    }

    public string GetCurrentLevelName()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        return collectibleStats[scene].GetLevelName();
    }
}
