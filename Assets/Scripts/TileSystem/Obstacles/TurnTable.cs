using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : Obstacle
{
    //Will update this when asset is created
    //[SerializeField] bool _turnsLeft;

    public override void PerformObstacleAnim()
    {
        _isActive = true;
        //if(_turnsLeft)
        //{
        //    //turn left anim
        //}
        //else
        {
            _anim.SetTrigger("Right");
        }
        _isActive = false;
    }

    public override void SetToDefaultState()
    {
        _isActive = false;
    }
}
