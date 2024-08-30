using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle", menuName = "Obstacle")]
public class Obstacle : ScriptableObject
{
    public enum ObstacleType
    {
        None, Arch, Ramp, Spring, Spike
    }
    [SerializeField] private ObstacleType obstacleType;
}
