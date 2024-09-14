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
        print("Fired win event");
        base.PerformObstacleAnim();
        GameManager.Instance.ChangeGameState(GameManager.STATE.End);
    }
    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
}
