/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: October 10, 2024
*    Description: This script is for the Speed Pad Obstacle. It needs no
*       additional functionality to the base class. 
*       
*       I didn't want to risk renaming this file, and replacing the script would mean
*       editing the direction of every ramp in every level again and so I decided to
*       just leave it like this.
*******************************************************************/
public class Ramp : Obstacle
{
    public override void PerformObstacleAnim()
    {
        base.PerformObstacleAnim();

        SfxManager.Instance.PlaySFX(6735);
    }
}