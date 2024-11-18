/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 8/29/24
*    Description: This manager will keep track of all tiles in the 
*    scene. The player or computer controller will need this frequently
*******************************************************************/
using System.Collections;
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
    private float increment = 0.001f;
    private float fallDuration = 0.25f;
    private float fallDistance = 7;
    private float delayBetweenFalls = 0.04f;
    [SerializeField] private List<Tile> allTilesInScene = new List<Tile>();
    [SerializeField] private Dictionary<Obstacle, Vector2> allObstacles = new Dictionary<Obstacle, Vector2>();
    [SerializeField] private Dictionary<Collectable, Vector2> allCollectables = new Dictionary<Collectable, Vector2>();
    Dictionary<GameObject, bool> gameObjectsToFall = new Dictionary<GameObject, bool>();
    // [SerializeField] private Dictionary<Collectable, Vector2> allCollectables = new Dictionary<Collectable, Vector2>();

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
    /// This method searches the scene for ALL tile objects as a setup method. 
    /// DO NOT USE THIS TO GET THE OBSTACLELIST
    /// </summary>
    public void LoadObstacleList()
    {
        var tilesWithObstacles = GetAllTilesInScene()
                             .Where(tile => tile.GetObstacleClass() != null);

        allObstacles.Clear();

        foreach (var tile in tilesWithObstacles)
        {
            Obstacle obstacle = tile.GetObstacleClass();
            Vector2 coordinates = tile.GetCoordinates();
            allObstacles.Add(obstacle, coordinates);
        }
    }
    /// <summary>
    /// This method searches the scene for ALL tile objects as a setup method. 
    /// DO NOT USE THIS TO GET THE COLLECTIBLELIST
    /// </summary>
    public void LoadCollectibleList()
    {
        allCollectables.Clear();
        var tilesWithCollectables = GetAllTilesInScene().Where(tile => tile.GetCollectableClass() != null);
        foreach (var tile in tilesWithCollectables)
        {
            Collectable collectible = tile.GetCollectableClass();
            Vector2 coordinates = tile.GetCoordinates();
            allCollectables.Add(collectible, coordinates);
        }
    }

    /// <summary>
    /// Gets all loaded tiles in this scene
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetAllTilesInScene()
    {
        return allTilesInScene;
    }
    public List<Obstacle> GetAllObstacles()
    {
        return allObstacles.Keys.ToList();
    }
    //public List<Collectable> GetCollectables()
    //{
    //    return allCollectables.Keys.ToList();
    //}
    public Obstacle GetObstacleWithTileCoordinates(Vector2 coordinates)
    {
        return allObstacles.FirstOrDefault(obstacle => obstacle.Value == coordinates).Key;
    }
    //public Collectable GetCollectableWithTileCoordinates(Vector2 coordinates)
    //{
    //    return allCollectables.FirstOrDefault(collectable => collectable.Value == coordinates).Key;
    //}

    public Tile[] GetTilesInLine(Tile originTile, Tile targetTile)
    {
        List<Tile> tilesInLine = new List<Tile>();

        Vector2 targetCoords = targetTile.GetCoordinates();
        Vector2 direction = (targetCoords - originTile.GetCoordinates()).normalized;

        // initialize the current position as starting from the tile adjacent to the origin tile
        Vector2 currentCoords = originTile.GetCoordinates() + direction;

        while (currentCoords != targetCoords)
        {
            Tile currentTile = GetTileByCoordinates(currentCoords);

            if (currentTile != null)
            {
                tilesInLine.Add(currentTile);
            }
            currentCoords += direction;
        }

        // add the target tile as the last tile to the list
        Tile targetTileInLine = GetTileByCoordinates(targetCoords);
        if (targetTileInLine != null)
        {
            tilesInLine.Add(targetTileInLine);
        }
        return tilesInLine.ToArray();
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
    public int GetDirectionBetweenTiles(Tile tileA, Tile tileB)
    {
        Vector2 coordA = tileA.GetCoordinates(); // Assuming Coordinates is a Vector2 property
        Vector2 coordB = tileB.GetCoordinates();

        if (coordA.x == coordB.x)
        {
            if (coordA.y < coordB.y) return 1; // North
            if (coordA.y > coordB.y) return 7; // South
        }
        else if (coordA.y == coordB.y)
        {
            if (coordA.x < coordB.x) return 5; // East
            if (coordA.x > coordB.x) return 3; // West
        }

        // Default or invalid input (e.g., non-adjacent tiles)
        return -1;
    }

    #endregion

    #region Setters
    #endregion    

    public void MoveObjectsToStartingPos()
    {
        List<GameObject> gameObjectsToFall = new List<GameObject>();
        GameObject player = FindObjectOfType<PlayerStateMachineBrain>(false).gameObject;
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y * fallDistance, player.transform.position.z);

        foreach (Tile tile in allTilesInScene) //adding all tiles to the dictionary
        {
            this.gameObjectsToFall.Add(tile.gameObject, false);
            tile.gameObject.transform.position = tile.gameObject.transform.position - Vector3.up * fallDistance;
        }

        gameObjectsToFall.AddRange(allObstacles.Keys.Select(obstacle => obstacle.gameObject));
        gameObjectsToFall.AddRange(allCollectables.Keys.Select(collectable => collectable.gameObject));
        gameObjectsToFall.AddRange(FindObjectsOfType<MovingWallController>().Select(go => go.gameObject));

        foreach (GameObject gameObject in gameObjectsToFall)//adding everything else to the dictionary
        {
            gameObject.transform.position = (gameObject.transform.position + Vector3.up * fallDistance);
            this.gameObjectsToFall.Add(gameObject, true);
        }
    }
    public IEnumerator FallAllGameObjects()
    {
        var count = 0;
        yield return new WaitForSeconds(0.5f); //waiting for initializations. Trust.
        foreach (var gameObject in this.gameObjectsToFall)
        {
            StartCoroutine(LerpingGameObjects(gameObject.Key, gameObject.Value));
            delayBetweenFalls -= increment;
            count++;
            if (count % 2 == 0 && count > 3)
            {
                ShakeManager.ShakeCamera(1.5f, 1, 0.25f);
            }
            yield return new WaitForSeconds(delayBetweenFalls);
        }
        yield return new WaitForSeconds(0.5f); //waiting for the last of the obstacles to fall before we set them to their assured position
        foreach (Tile tile in allTilesInScene)
        {
            tile.TryMoveObstacle();
            tile.TryMoveCollectable();
        }
        PlayerController.StartPlayerFall?.Invoke();
        yield return new WaitForSeconds(1f); //Waiting for the player to finish falling before letting them play cards
        GameManager.Instance.ChangeGameState(GameManager.STATE.ChooseCards);
    }

    private IEnumerator LerpingGameObjects(GameObject gameObject, bool fallingDown)
    {
        Vector3 startPos;
        Vector3 endPos;
        if (fallingDown)
        {
            startPos = gameObject.transform.position; // Starting position
            endPos = gameObject.transform.position - Vector3.up * fallDistance; // Final position
        }
        else
        {
            startPos = gameObject.transform.position; // Starting position
            endPos = gameObject.transform.position + Vector3.up * fallDistance; // Final position
        }


        gameObject.transform.position = startPos; // Set the starting position
        float elapsedTime = 0;

        while (elapsedTime < fallDuration)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = endPos; // Ensure it ends exactly at the final position
    }
    public void InitializeTileManager()
    {
        LoadTileList();
        LoadObstacleList();
        LoadCollectibleList();
        MoveObjectsToStartingPos();
    }
}
