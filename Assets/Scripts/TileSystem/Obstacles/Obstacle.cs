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
    [SerializeField] protected bool _isActive;
    [Tooltip(" North is positive on the z axis. East is positive on the x axis")][SerializeField] private Direction direction;

    public enum Direction
    {
        None, Northwest, North, Northeast, West, East, Southwest, South, Southeast
    }
    //Is this useful?
    public enum ObstacleType
    {
        None, Arch, Ramp, Spring, Spike
    }
    //[SerializeField] private ObstacleType obstacleType;

    private void OnEnable()
    {
        GameManager.DeathAction += SetToDefaultState;
        GameManager.TrapAction += PerformObstacleAnim;
    }
    private void OnDisable()
    {
        GameManager.DeathAction -= SetToDefaultState;
        GameManager.TrapAction -= PerformObstacleAnim;
    }

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
        _isActive = _defaultState;
    }
    public int GetDirection()
    {
        switch (direction)
        {
            case Direction.Northwest:
                return 0;
            case Direction.North:
                return 1;
            case Direction.Northeast:
                return 2;
            case Direction.West:
                return 3;
            case Direction.East:
                return 5;
            case Direction.Southwest:
                return 6;
            case Direction.South:
                return 7;
            case Direction.Southeast:
                return 8;
            default:
                return 4;
        }
    }
}
