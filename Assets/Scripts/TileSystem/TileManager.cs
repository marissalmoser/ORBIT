/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 8/29/24
*    Description: This manager will keep track of all tiles in the 
*    scene. The player or computer controller will need this frequently
*******************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    #region Singleton
    public static TileManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }
    #endregion
    [SerializeField] private List<Tile> allTilesInScene = new List<Tile>();

    public enum TileDirection
    {
        Northwest, North, Northeast, West, None, East, SouthWest, South, SouthEast,
    }
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
    public Tile GetTileAtLocation(Tile startTile, int direction, int distance)
    {
        Vector2[] directionOffsets = new Vector2[]
        {
        new Vector2(-1, 1),  // 0: Northwest
        new Vector2(0, 1),   // 1: North
        new Vector2(1, 1),   // 2: Northeast
        new Vector2(-1, 0),  // 3: West
        new Vector2(0, 0),   // 4: None (stay at the same tile)
        new Vector2(1, 0),   // 5: East
        new Vector2(-1, -1), // 6: Southwest
        new Vector2(0, -1),  // 7: South
        new Vector2(1, -1),  // 8: Southeast
        };

        Vector2 startCoords = startTile.GetCoordinates(); 
        Vector2 offset = directionOffsets[direction] * distance;
        Vector2 targetCoords = startCoords + offset;

        Tile targetTile = GetAllTilesInScene().FirstOrDefault(tile => tile.GetCoordinates() == targetCoords);

        return targetTile;
    }
    public Tile GetTileByCoordinates(Vector2 coordinates)
    {
        Tile tile = allTilesInScene.FirstOrDefault(t => t.GetCoordinates() == coordinates);
        if (tile != null)
        {
            return tile;
        }
        Debug.LogError("Could not find a tile in the tilelist at coordinates " + coordinates);
        return null;
    }
    #endregion

    #region Setters
    #endregion    
}
