/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 10, 2024
*    Description: 
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Obstacle
{

    //spike card being a death card?


    public override void PerformObstacleAnim()
    {
        if(!_isActive)
        {
            _anim.SetTrigger("SpikeDown");
        }
        else
        {
            _anim.SetTrigger("SpikeUp");
        }
    }


    public override void SetToDefaultState()
    {
        _isActive = _defaultState;

        if (_defaultState)
        {
            _anim.SetTrigger("SpikeUp");
        }
        else
        {
            _anim.SetTrigger("SpikeDown");
        }
    }
}
