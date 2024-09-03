/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: August 31, 2024
*    Description: A custom editor window to build a map of tiles.
*******************************************************************/
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;

public class TileMapBuilder : EditorWindow
{
    Vector2Int _mapSize = new Vector2Int(3,3);
    int _emptyBorderSize = 3;
    GameObject _defaultTilePrefab;
    GameObject _tileParentObj;

    /// <summary>
    /// Function that allows the Tile Map Builder to have its own window. Access it
    /// by going to the menu/tools/Tile Map Builder.
    /// </summary>
    [MenuItem("Tools/Tile Map Builder")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TileMapBuilder));
    }

    /// <summary>
    /// Sets up the fields and buttons in the custom window.
    /// </summary>
    private void OnGUI()
    {
        //serialized fields
        _mapSize = EditorGUILayout.Vector2IntField("Map Size", _mapSize);

        _emptyBorderSize = EditorGUILayout.IntField("Empty Boarder Size", _emptyBorderSize);

        _defaultTilePrefab = EditorGUILayout.ObjectField("Default Tile Prefab",
            _defaultTilePrefab, typeof(GameObject), false) as GameObject;

        _tileParentObj = EditorGUILayout.ObjectField("Tile Parent",
            _tileParentObj, typeof(GameObject), true) as GameObject;

        //buttons
        EditorGUILayout.Space();
        if (GUILayout.Button("Create New Map"))
        {
            CreateNewTileMap();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Clear Map"))
        {
            ClearTileMap();
        }
    }

    /// <summary>
    /// Function to create a new map of tiles based on _mapSize with a border of
    /// empty "hole" tiles around it.
    /// </summary>
    private void CreateNewTileMap()
    {
        ClearTileMap();

        for (int i = -_emptyBorderSize; i < _mapSize.x + _emptyBorderSize; i++)
        {
            for (int j = -_emptyBorderSize; j < _mapSize.y + _emptyBorderSize; j++)
            {
                //Create tiles for map
                GameObject go = Instantiate(_defaultTilePrefab, new Vector3(i, 0, j),
                    Quaternion.identity, _tileParentObj.transform);

                //Set up tile's fields
                Tile tile = go.GetComponent<Tile>();
                tile.SetCoordinates(new Vector2(i, j));

                //makes hole tiles around the map
                if (i < 0 || i >= _mapSize.x || j < 0 || j >= _mapSize.y)
                {
                    tile.SetTileType(Tile.TileType.Hole);
                }
            }
        }
    }

    /// <summary>
    /// Clears the current tile map.
    /// </summary>
    private void ClearTileMap()
    {
        //ensures the tile parent is not null
        if(_tileParentObj == null)
        {
            Debug.LogError("Tile Parent field is null; Drag a game object into the inspector");
            return;
        }

        Transform[] tiles = _tileParentObj.GetComponent<Transform>()
            .GetComponentsInChildren<Transform>(true);

        for(int i = tiles.Length - 1; i > 0; i--)
        {
            if(tiles[i].gameObject != _tileParentObj)
            {
                DestroyImmediate(tiles[i].gameObject);
            }
        }
    }
}
