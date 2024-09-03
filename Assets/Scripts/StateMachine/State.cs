/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 9/1/24
*    Description: 
*******************************************************************/
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
        //Tell tile to do a trap animation

        //Tell player to do its animation
        throw new System.NotImplementedException();
    }
}
public class EvaluateTargetTile : State
{
    private Tile currentTile;
    private Tile targetTile;
    public EvaluateTargetTile()
    {
        targetTile = TileManager.Instance.GetTileAtLocation(currentTile, 2, 2);
    }
    public override State Execute(StateMachine context)
    {
        Tile tile = new Tile();
        if (targetTile.IsHole())
        {
            //fall into the abyss, sucker
        }
        else
        {
            //go to tile
        }
        return new EvaluateLocalTile(tile);
    }
}

public class EvaluateLocalTile : State
{
    private Tile localTile;
    public EvaluateLocalTile(Tile thisTile)
    {
        localTile = thisTile; 
    }
    public override State Execute(StateMachine context)
    {
        return new EvaluateTargetTile();
    }
}

public class ExitStateMachine : State
{
    public override State Execute(StateMachine context)
    {
        context.EndStateMachine();
        return null;
    }
}