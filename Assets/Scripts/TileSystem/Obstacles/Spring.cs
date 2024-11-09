/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: Springs are activated at the end of overy other turn.
*******************************************************************/
using UnityEngine;

public class Spring : Obstacle
{
    //dummy bool for springs cus they were acting up smth silly - i checked again :(
    [SerializeField] private bool _active;

    private bool _hasFired;

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
            ShakeManager.ShakeCamera(0.5f, 0.5f, 0.3f);
            _anim.SetTrigger("SpringUp");
            _hasFired = true;
        }
    }

    public override void SwitchActiveState()
    {
        _active = !_active;
        if (_active)
        {
            _anim.SetBool("IsActive", true);
        }
        else
        {
            if(!_hasFired)
            {
                _anim.SetTrigger("SpringUp");
            }
            _anim.SetBool("IsActive", false);
        }
        _hasFired = false;
    }

    public override void SetToDefaultState()
    {
        _active = _defaultState;
        if(_active )
        {
            _anim.SetBool("IsActive", true);
        }
    }
}
