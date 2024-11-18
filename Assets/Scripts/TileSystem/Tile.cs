/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: August 31, 2024
*    Description: This script will be on all tiles. It contains the functionality to
*    assign obstacles and collectables to a tile and moves the objects to it's anchor.
*******************************************************************/
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Tile, Hole
    }

    [Tooltip("Do not edit this field, it is just serialized for reference")]
    [SerializeField] private Vector2 _coordinates;
    [SerializeField] private TileType _tileType;
    [SerializeField] private int _elevation;
    private TileType _lastTileType;

    [SerializeField] private GameObject _obstacleRef;
    private Obstacle _obstacleBehavior;
    private GameObject _obstacleAnchor;

    [SerializeField] private GameObject _collectableRef;
    private Collectable _collectableBehavior;
    private GameObject _collectableAnchor;

    private GameObject _playerSnapTo;

    #region GettersAndSetters

    public void Start()
    {
        _playerSnapTo = GetPlayerSnapAnchor();
        //TryMoveObstacle();
        //TryMoveCollectable();

        //sets tile type
        if (_tileType != _lastTileType)
        {
            SetTileType(_tileType);
            _lastTileType = _tileType;
        }
        if (_tileType == TileType.Hole)
        {
            _elevation = 0;
        }
    }
    /// <summary>
    /// Returns a tile's 2D coordinates on the map
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }

    /// <summary>
    /// Sets a tile's 2D corrdinates on the map. This should only be called from the 
    /// tile map builder.
    /// </summary>
    /// <param name="coordinates"></param>
    public void SetCoordinates(Vector2 coordinates)
    {
        this._coordinates = coordinates;
    }

    /// <summary>
    /// Checks if the tile is a hole, returns true if it is.
    /// </summary>
    /// <returns></returns>
    public bool IsHole()
    {
        if (_tileType == TileType.Hole)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the tile's obstacle anchor from the transform's list of children.
    /// </summary>
    /// <returns></returns>
    public GameObject GetObstacleAnchor()
    {
        if (_obstacleAnchor != null)
        {
            return _obstacleAnchor;
        }
        if (transform.GetChild(0).gameObject != null)
        {
            _obstacleAnchor = transform.GetChild(0).gameObject;
            _obstacleBehavior = _obstacleRef.GetComponent<Obstacle>();
            return _obstacleAnchor;
        }

        Debug.LogError("Obstical Anchor is null");
        return null;
    }

    public int GetElevation()
    {
        return _elevation;
    }
    /// <summary>
    /// returns the reference to the tile's obstacle class
    /// </summary>
    /// <returns></returns>
    public Obstacle GetObstacleClass()
    {
        if(_obstacleBehavior != null)
        {
            return _obstacleBehavior;
        }
        return null;
    }

    /// <summary>
    /// Gets the tile's collectable anchor from the transform's list of children.
    /// </summary>
    /// <returns></returns>
    public GameObject GetCollectableAnchor()
    {
        if(_collectableAnchor != null)
        {
            return _collectableAnchor;
        }
        if(transform.GetChild(1).gameObject != null)
        {
            _collectableAnchor = transform.GetChild(1).gameObject;
            _collectableBehavior = _collectableRef.GetComponent<Collectable>();
            return _collectableAnchor;
        }

        Debug.LogError("Collectable Anchor is null");
        return null;
    }

    /// <summary>
    /// returns the reference to the tile's collectable class
    /// </summary>
    /// <returns></returns>
    public Collectable GetCollectableClass()
    {
        if (_collectableBehavior != null)
        {
            return _collectableBehavior;
        }

        //Debug.LogError("collectable behvaior is null");
        return null;
    }

    /// <summary>
    /// Gets the tile's player snap anchor from the transform's list of children.
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerSnapAnchor()
    {
        if(_playerSnapTo != null)
        {
            return _playerSnapTo;
        }
        if (transform.GetChild(2).gameObject != null)
        {
            _playerSnapTo = transform.GetChild(2).gameObject;
            return _playerSnapTo;
        }

        Debug.LogError("Player snap is null");
        return null;
    }

    /// <summary>
    /// Returns the Vector 3 position of the tile's player snap to anchor.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerSnapPosition()
    {
        if(_playerSnapTo != null)
        {
            return _playerSnapTo.transform.position;
        }
        return GetPlayerSnapAnchor().transform.position;
    }

    #endregion

    #region EditorFunctions

    /// <summary>
    /// When a field in the inspector is updated, updates the scene view.
    /// </summary>
    public void OnValidate()
    {
        TryMoveObstacle();
        TryMoveCollectable();

        //sets tile type
        if (_tileType != _lastTileType)
        {
            SetTileType(_tileType);
            _lastTileType = _tileType;
        }
        if(_tileType == TileType.Hole)
        {
            _elevation = 0;
        }
        //TODO : add handler for blocks on tile
    }

    /// <summary>
    /// If there is an obstacle in the tile's obstacle ref field, move it to it's 
    /// position above the tile.
    /// </summary>
    public void TryMoveObstacle()
    {
        if (_obstacleRef != null)
        {
            //ensures the object in the obstacle ref field is of correct type
            if (_obstacleRef.TryGetComponent<Obstacle>(out _) == false)
            {
                Debug.LogError("Obstacle ref is not of type Obstable");
                _obstacleRef = null;
                return;
            }
            //get and move obstacle ref to the anchor point
            _obstacleRef.transform.position = GetObstacleAnchor().transform.position;
        }     
    }

    /// <summary>
    /// If there is a collectable in the tile's collectable ref field, move it to it's 
    /// position above the tile.
    /// </summary>
    public void TryMoveCollectable()
    {
        if (_collectableRef != null)
        {
            //ensures the object in the collectable ref field is of correct type
            if (_collectableRef.TryGetComponent<Collectable>(out _) == false)
            {
                Debug.LogError("Collectable ref is not of type Collectable");
                _collectableRef = null;
                return;
            }

            //get and move collectable ref to the anchor point
            _collectableRef.transform.position = GetCollectableAnchor().transform.position;
        }
    }

    /// <summary>
    /// Sets the tile to visible based on parameter tile type.
    /// </summary>
    public void SetTileType(TileType type)
    {
        if(type == TileType.Tile)
        {
            _tileType = TileType.Tile;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            _tileType = TileType.Hole;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
    public void SetElevation(int height)
    {
        _elevation = height;
    }

    #endregion

}