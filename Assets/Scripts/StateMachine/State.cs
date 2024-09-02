/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/1/24
*    Description: 
*******************************************************************/

using UnityEngine;

/// <summary>
/// Base class all classes inherit from
/// </summary>
public abstract class State
{
    public abstract State Execute(StateMachine context);
}

/// <summary>
/// 
/// </summary>
public class PlayActionAnimation : State
{
    public override State Execute(StateMachine context)
    {
        throw new System.NotImplementedException();
    }
}
public class EvaluateFacingSquare : State
{
    public override State Execute(StateMachine context)
    {
        Tile tile = new Tile();
        return new EvaluateLocalTile(tile);
    }
}

public class EvaluateLocalTile : State
{
    private Tile tile;
    public EvaluateLocalTile(Tile thisTile)
    {
        tile = thisTile; 
    }
    public override State Execute(StateMachine context)
    {
        return new EvaluateFacingSquare();
    }
}