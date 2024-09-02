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
        throw new System.NotImplementedException();
    }
}
public class EvaluateFacingSquare : State
{
    public override State Execute(StateMachine context)
    {
        throw new System.NotImplementedException();
    }
}