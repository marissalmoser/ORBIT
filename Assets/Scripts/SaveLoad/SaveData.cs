/******************************************************************
*    Author: Elijah Vroman
*    Contributors: Elijah Vroman,
*    Date Created: 10/27/24
*    Description: 
*******************************************************************/
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<LevelData> levelInformation = new List<LevelData>();
    public SaveData()
    {
        levelInformation = new List<LevelData>();
    }
}

[System.Serializable]
public class LevelData
{
    [SerializeField] bool isCollected;
    [SerializeField] bool isCompleted;
    [SerializeField] bool isLocked;
    [SerializeField] string levelName;
    public LevelData()
    {
        levelName = string.Empty;
        isCollected = false;
        isLocked = true;
        isCompleted = false;
    }

    public string GetLevelName()
    { 
        return levelName; 
    }
    public bool GetIsCollected()
    {
        return isCollected;
    }
    public bool GetIsLocked()
    {
        return isLocked;
    }
    public bool GetIsCompleted()
    {
        return isCompleted;
    }
    public void SetName(string name)
    {
        levelName = name;
    }
    public void SetCollectedStatus(bool status)
    {
        isCollected = status;
    }
    public void SetLockedStatus(bool status)
    {
        isLocked=status;
    }
    public void SetCompletedStatus(bool status)
    {
        isCompleted = status;
    }
}