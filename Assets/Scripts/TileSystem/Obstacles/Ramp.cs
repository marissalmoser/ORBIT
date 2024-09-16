public class Ramp : Obstacle
{
    public override void PerformObstacleAnim()
    {
        if (_isActive)
        {
            _anim.SetTrigger("Ramp Down");
        }
        else
        {
            _anim.SetTrigger("Ramp Up");
        }
    }


    public override void SetToDefaultState()
    {
        _isActive = _defaultState;

        if (_defaultState)
        {
            _anim.SetTrigger("Ramp Up");
        }
        else
        {
            _anim.SetTrigger("Ramp Down");
        }
    }
}