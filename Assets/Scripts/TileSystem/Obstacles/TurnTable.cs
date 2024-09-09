using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : Obstacle
{
    [SerializeField] bool _turnsLeft;

    public override void PerformObstacleAnim()
    {
        _isActive = true;
        if(_turnsLeft)
        {
            //turn left anim
        }
        else
        {
            //turn right anim
        }
        _isActive = false;
    }

    public override void SetToDefaultState()
    {
        _isActive = false;
    }
}
