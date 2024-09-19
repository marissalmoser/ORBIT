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
        //sound effect call
        GameObject manager = GameObject.Find("SfxManager");
        SfxManager function_call = (SfxManager)manager.GetComponent(typeof(SfxManager));

        if (!_isActive)
        {
            function_call.PlaySFX(1987);
            _anim.SetTrigger("SpikeDown");
        }
        else
        {
            function_call.PlaySFX(4136);
            _anim.SetTrigger("SpikeUp");
        }
    }


    public override void SetToDefaultState()
    {
        _isActive = _defaultState;

        GameObject manager = GameObject.Find("SfxManager");
        SfxManager function_call = (SfxManager)manager.GetComponent(typeof(SfxManager));

        if (_isActive)
        {
            function_call.PlaySFX(4136);
            _anim.SetTrigger("SpikeUp");
        }
        else
        {
            function_call.PlaySFX(1987);
            _anim.SetTrigger("SpikeDown");
        }
    }
}
