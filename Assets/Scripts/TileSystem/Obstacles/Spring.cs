/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: Springs are activated at the end of overy other turn.
*******************************************************************/
using UnityEngine;

public class Spring : Obstacle
{
    //dummy bool for springs cus they were acting up smth silly
    [SerializeField] private bool _active;

    private void OnEnable()
    {
        GameManager.TrapAction += SwitchActiveState;
    }
    private void OnDisable()
    {
        GameManager.TrapAction -= SwitchActiveState;
    }

    public override bool IsActive()
    {
        return _active;
    }

    public override void PerformObstacleAnim()
    {
        if (_active)
        {
            _anim.SetTrigger("SpringUp");
        }
        else
        {
            _anim.SetTrigger("SpringReady");
        }
    }

    public override void SwitchActiveState()
    {
        _active = !_active;
        PerformObstacleAnim();
    }

    public override void SetToDefaultState()
    {
        _active = _defaultState;
        _isActive = _defaultState;
        _anim.SetTrigger("SpringReady");
    }
}
