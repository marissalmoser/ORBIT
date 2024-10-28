/******************************************************************
*    Author: Elijah Vroman
*    Contributors: Elijah Vroman
*    Date Created: 20/27/24
*    Description: This manager allows saving and loading, assigning 
*    savedata, deleting savedata, etc
*******************************************************************/
using System;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    #region Instance
    //regions are cool, i guess. Just hiding boring stuff
    public static SaveLoadManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion
    public event Action OnSaveGame;
    public event Action<SaveData> OnLoadData;

    private const string directory = "/SavedGames/";
    private const string fileName1 = "SaveGame1.sav";
    private const string fileName2 = "SaveGame2.sav";
    private const string fileName3 = "SaveGame3.sav";

    private SaveData newData = new SaveData();

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            SaveDataToFile(1);
            
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            DeleteSaveData(1);

        }
    }

    private void CollectLevelSaveData()
    {
        newData = new SaveData();
        foreach (CollectibleStats entry in CollectibleManager.Instance.collectibleStats)
        {
            newData.levelInformation.Add(entry);
        }
    }

    public void SaveDataToFile(int fileToSaveTo)
    {
        string dir = Application.persistentDataPath + directory;
        string fullPath = Application.persistentDataPath + directory + GetFileNameByInt(fileToSaveTo);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            Debug.Log("Directory Created");
        }

        CollectLevelSaveData();
        string jsonString = JsonUtility.ToJson(newData, true);

        File.WriteAllText(dir + GetFileNameByInt(fileToSaveTo), jsonString);
        GUIUtility.systemCopyBuffer = dir;
        Debug.Log("Saved to savefile " + fileToSaveTo);
    }

    public SaveData LoadDataFromFile(int fileToLoad)
    {
        Debug.Log("File " + GetFileNameByInt(fileToLoad) +  " saved");
        return newData;
    }
    /// <summary>
    /// Used by external scripts to confirm if saved data exists
    /// </summary>
    /// <returns>True if there's saved data</returns>
    public bool DoesSaveFileExist(int fileToCheck)
    {
        string fullPath = Application.persistentDataPath + directory + GetFileNameByInt(fileToCheck);

        if (File.Exists(fullPath))
        {
            Debug.Log("File " + GetFileNameByInt(fileToCheck) + " found");
            return true;
        }
            
        else
        {
            Debug.Log("File " + GetFileNameByInt(fileToCheck) + " NOT found");
            return false;
        }

    }
    /// <summary>
    /// Deletes saved data if it exists
    /// </summary>
    public void DeleteSaveData(int fileToDelete)
    {
        string fullPath = Application.persistentDataPath + directory + GetFileNameByInt(fileToDelete);
        if (File.Exists(fullPath))//if a file exists at this path
        {
            Debug.Log("File " + GetFileNameByInt(fileToDelete) + " deleted");
            File.Delete(fullPath);
        }
        else
        {
            Debug.Log("File " + GetFileNameByInt(fileToDelete) + "NOT deleted; could not be found");
        }
    }
    /// <summary>
    /// Gets the const string fileName with an int, because we will have 3 save files available
    /// </summary>
    /// <param name="getter"></param>
    /// <returns></returns>
    private string GetFileNameByInt(int getter)
    {
        switch (getter)
        {
            case 1:
                return fileName1;
            case 2:
                return fileName2;
            case 3:
                return fileName3;
            default:
                Debug.LogError("Something went wrong by trying to get the const string filename with an int. The wrong int was inputted");
                return null;
        }
    }
}
