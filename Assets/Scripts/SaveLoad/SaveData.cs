/******************************************************************
*    Author: Elijah Vroman
*    Contributors: Elijah Vroman,
*    Date Created: 10/27/24
*    Description: 
*******************************************************************/
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<CollectibleStats> levelInformation = new List<CollectibleStats>();
    public SaveData()
    {
        levelInformation = new List<CollectibleStats>();
    }
}