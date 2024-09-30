/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: Springs are activated at the end of overy other turn.
*******************************************************************/
public class Spring : Obstacle
{
    private void OnEnable()
    {
        GameManager.TrapAction += SwitchActiveState;
    }
    private void OnDisable()
    {
        GameManager.TrapAction -= SwitchActiveState;
    }
    public override void PerformObstacleAnim()
    {
        if (_isActive)
        {
            _anim.SetTrigger("SpringUp");
        }
    }

    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
}
