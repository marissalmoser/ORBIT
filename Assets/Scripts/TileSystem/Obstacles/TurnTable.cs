/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: Turn tables turn at the end of every turn.
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : Obstacle
{
    [SerializeField] bool _turnsLeft;

    public override void PerformObstacleAnim()
    {
        if(_isActive)
        {
            if (_turnsLeft)
            {
                _anim.SetTrigger("Left");
            }
            else
            {
                _anim.SetTrigger("Right");
            }
        }
    }

    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
}
