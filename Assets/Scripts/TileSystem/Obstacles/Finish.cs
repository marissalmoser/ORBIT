/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: This will call a finish level 
*******************************************************************/
using UnityEngine;
public class Finish : Obstacle
{
    public override void PerformObstacleAnim()
    {
        //TODO : invoke win event
        print("Fired win event");
        base.PerformObstacleAnim();
    }
    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
}
