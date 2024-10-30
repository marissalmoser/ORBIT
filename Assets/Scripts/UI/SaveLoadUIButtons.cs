/******************************************************************
*    Author: Elijah Vroman
*    Contributors: Elijah Vroman
*    Date Created: 10/28/24
*    Description: This script lets UI TMPro buttons communicate with
*    the saveloadmanager, because fields lose their reference to
*    a manager that persists between sceens. Only for main menu.
*    SaveLoadManager holds which save the player is currently on, so
*    that menu tabbing doesnt lose track of that value
*******************************************************************/
using UnityEngine;

public class SaveLoadUIButtons : MonoBehaviour
{
    public void LoadCurrentSave()
    {
        SaveLoadManager.Instance.LoadDataFromFile(SaveLoadManager.Instance.GetCurrentSaveSelected());
    }
    public void LoadSave(int save)
    {
        SaveLoadManager.Instance.LoadDataFromFile(save);
    }

    /// <summary>
    /// Since this is never called unless SaveToDeleteSelected is called first, 
    /// saveSelected should always be the correct save to delete. Buttons assign
    /// saveSelectedInUI
    /// </summary>
    public void DeleteSave()
    {
        SaveLoadManager.Instance.DeleteSaveData(SaveLoadManager.Instance.GetCurrentSaveSelected());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="saveSelectedByPlayer"></param>
    public void SetSelectedSave(int saveSelectedByPlayer)
    {
        if (saveSelectedByPlayer <= 0 || saveSelectedByPlayer > 3)
        {
            Debug.LogError("SetSelectedSave() set a number that was not a valid save file number. " +
                "Something is setting saveSelected to: " + saveSelectedByPlayer);
        }
        SaveLoadManager.Instance.SetCurrentSaveSelected(saveSelectedByPlayer);
    }
}
