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

    public bool IsActive()
    {
        return _isActive;
    }

    public void SetIsActive(bool input)
    {
        _isActive = input;
    }

    public void SetDefaultState(bool input)
    {
        _defaultState = input;
    }

    public Card GetCard()
    {
        return _obstacleMovementCard;
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

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.P))
        {
            PerformObstacleAnim();
        }
    }
}
