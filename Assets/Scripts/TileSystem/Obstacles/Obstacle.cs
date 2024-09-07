/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 2, 2024
*    Description: This is the base class for Obstacles.
*******************************************************************/
using UnityEngine;

[SelectionBase]
public abstract class Obstacle : MonoBehaviour
{
    [SerializeField] protected Card _obstacleMovementCard;
    protected Animator _anim;

    [SerializeField] protected bool _defaultState;
    protected bool _isActive;

    //Is this useful?
    public enum ObstacleType
    {
        None, Arch, Ramp, Spring, Spike
    }
    //[SerializeField] private ObstacleType obstacleType;

    private void Start()
    {
        _anim = GetComponent<Animator>();

        SetToDefaultState();
    }

    public bool getIsActive()
    {
        return _isActive;
    }

    /// <summary>
    /// Called in between each turn
    /// </summary>
    public virtual void PerformObstacleAnim()
    {

    }

    /// <summary>
    /// Call this to set the obstacle to it's default state, it's state on the first
    /// turn.
    /// </summary>
    public virtual void SetToDefaultState()
    {

    }
}
