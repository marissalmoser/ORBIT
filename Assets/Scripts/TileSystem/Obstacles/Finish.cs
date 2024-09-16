/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: This will call a finish level 
*******************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Finish : Obstacle
{
    private PlayerController _pC;
    private Tile _tileFinishIsOn;

    public void Start()
    {
        _pC = FindObjectOfType<PlayerController>();
        _tileFinishIsOn = TileManager.Instance.GetAllTilesInScene().FirstOrDefault(tile => tile.GetObstacleClass() is Finish);
    }
    public override void PerformObstacleAnim()
    {
        if ((_tileFinishIsOn != null) && _tileFinishIsOn == _pC.GetCurrentTile())
        {
            print("Fired win event");
            base.PerformObstacleAnim();
            GameManager.Instance.ChangeGameState(GameManager.STATE.End);
        } 
    }
    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
}
