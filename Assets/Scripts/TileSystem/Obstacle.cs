/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 2, 2024
*    Description: This is the base class for Obstacles.
*******************************************************************/
using UnityEngine;

[SelectionBase]
public class Obstacle : MonoBehaviour
{
    //Is this useful?
    //public enum ObstacleType
    //{
    //    None, Arch, Ramp, Spring, Spike
    //}
    //[SerializeField] private ObstacleType obstacleType;

    /// <summary>
    /// Called in between each turn and when the player lands on it
    /// </summary>
    public virtual void PerformObstacleAnim()
    {

    }
}
