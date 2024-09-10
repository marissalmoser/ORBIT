using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : Obstacle
{
    public override void PerformObstacleAnim()
    {
        if (_isActive)
        {
            _anim.SetTrigger("Spring");
        }
        _isActive = !_isActive;
    }


    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
}
