/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/1/24
*    Description: 
*******************************************************************/
using UnityEngine.Events;
using UnityEngine;


/// <summary>
/// Base class all classes inherit from
/// </summary>
public abstract class State
{
    public abstract State Execute(StateMachineOld context);
}





/// <summary>
/// Complete the action; play the animation
/// </summary>
public class PlayActionAnimation : State
{
    public override State Execute(StateMachineOld context)
    {
        //Tell tile to do a trap animation

        //Tell player to do its animation
        throw new System.NotImplementedException();
    }
}

/// <summary>
/// Can i complete the action to the passed tile?
/// </summary>
public class EvaluateAction : State
{
    private Tile tile;
    private Card action;
    public EvaluateAction( Tile targetTile)
    {
        tile = targetTile;
    }
    public override State Execute(StateMachineOld context)
    {
        action = context.GetCurrentAction();
        switch(action.name)
        {
            case Card.CardName.Move:
                break;
            case Card.CardName.Jump:
                break;
            case Card.CardName.Turn:
                break;
            default:
                Debug.LogError("Not an expected valid card");
                break;
        }
        return null;
    }
}

/// <summary>
/// Whats on the tile?
/// </summary>
public class EvaluateTargetTile : State
{
    private Tile currentTile;
    private Tile targetTile;
    public override State Execute(StateMachineOld context)
    {
        currentTile = context.GetPlayerController().GetCurrentTile();
        int facingDirection = context.GetPlayerController().GetCurrentFacingDirection();
        int distance = context.GetCurrentAction().GetDistance();
        targetTile = TileManager.Instance.GetTileAtLocation(currentTile, facingDirection, distance);
        var obstacleOnTile = targetTile.GetObstacleAnchor();

        if (targetTile.IsHole())
        {
            
        }
        else if (obstacleOnTile != null)
        {
            //evaluate whats on the tile
        }
        else
        {
            //just move to tile
        }
        return new EvaluateAction(targetTile);
    }
}

/// <summary>
/// After moving to this tile, what happens?
/// </summary>
public class EvaluateLocalTile : State
{
    private Tile localTile;
    public EvaluateLocalTile(Tile thisTile)
    {
        localTile = thisTile; 
    }
    public override State Execute(StateMachineOld context)
    {
        return new EvaluateTargetTile();
    }
}

public class ExitStatemachine : State
{
    public override State Execute(StateMachineOld context)
    {
        context.GetNextAction();
        return null;
    }
}
public class ExitAndFail : State
{
    public override State Execute(StateMachineOld context)
    {
        context.EndStateMachine();
        return null;
    }
}