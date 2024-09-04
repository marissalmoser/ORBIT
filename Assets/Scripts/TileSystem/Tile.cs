using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int elevation;
    [SerializeField] private Vector2 coordinates;
    [SerializeField] private List<Obstacle> obstaclesInTile = new List<Obstacle>();
    [SerializeField] private Transform playerSnapTo;
    [SerializeField] private Mesh obstacleMesh;
    

    public bool IsHole()
    {
        return elevation < 0;
    }
    public int GetElevation()
    {
        return elevation;
    }
    public Vector2 GetCoordinates()
    {
        return coordinates;
    }
    public Transform GetPlayerSnap()
    {
        if(playerSnapTo != null)
        {
            return playerSnapTo;
        }
        Debug.LogError("Player snap is null; assign in inspector");
        return null;
    }
}