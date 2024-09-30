/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 10, 2024
*    Description: 
*******************************************************************/
using UnityEngine;

public class Spike : Obstacle
{
    private void OnEnable()
    {
        GameManager.TrapAction += SwitchActiveState;
    }
    private void OnDisable()
    {
        GameManager.TrapAction -= SwitchActiveState;
    }
    //spike card being a death card?


    public override void PerformObstacleAnim()
    {

        if (!_isActive)
        {
            SfxManager.Instance.PlaySFX(1987);
            _anim.SetTrigger("SpikeDown");
        }
        else
        {
            SfxManager.Instance.PlaySFX(4136);
            _anim.SetTrigger("SpikeUp");
        }
    }


    public override void SetToDefaultState()
    {
        _isActive = _defaultState;

        GameObject manager = GameObject.Find("SfxManager");
        SfxManager function_call = (SfxManager)manager.GetComponent(typeof(SfxManager));

        if (_isActive)
        {
            SfxManager.Instance.PlaySFX(4136);
            _anim.SetTrigger("SpikeUp");
        }
        else
        {
            SfxManager.Instance.PlaySFX(1987);
            _anim.SetTrigger("SpikeDown");
        }
    }
}
