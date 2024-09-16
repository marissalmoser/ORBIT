/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: Springs are activated at the end of overy other turn.
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : Obstacle
{
    public override void PerformObstacleAnim()
    {
        if (_isActive)
        {
            _anim.SetTrigger("SpringUp");
        }
    }

    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
}
