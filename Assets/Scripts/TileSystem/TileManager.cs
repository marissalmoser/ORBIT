/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 8/29/24
*    Description: This manager will keep track of all tiles in the 
*    scene. The player or computer controller will need this frequently
*******************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private List<Tile> allTilesInScene = new List<Tile>();

    #region Getters

    /// <summary>
    /// This method searches the scene for ALL tile objects as a setup method. 
    /// DO NOT USE THIS TO GET THE TILELIST
    /// </summary>
    public void LoadTileList()
    {
        allTilesInScene.Clear();
        allTilesInScene = FindObjectsByType<Tile>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).ToList();
    }
    /// <summary>
    /// Gets all loaded tiles in this scene
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetAllTilesInScene()
    {
        return allTilesInScene;
    }
    public Tile GetTileByCoordinates(Vector2 coordinates)
    {
        Tile tile = allTilesInScene.FirstOrDefault(t => t.GetCoordinates() == coordinates);
        if(tile != null)
        {
            return tile;
        }
        Debug.LogError("Could not find a tile in the tilelist at coordinates " + coordinates);
        return null;
    }
    #endregion

    #region Setters
    #endregion
    //public void Start()
    //{
    //    LoadTileList();
    //}
}
