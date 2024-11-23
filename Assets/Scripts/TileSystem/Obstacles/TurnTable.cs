/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 11, 2024
*    Description: Turn tables turn at the end of every turn.
*******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : Obstacle
{
    [SerializeField] bool _turnsLeft;
    [SerializeField] private GameObject _turnAnchor;
    [SerializeField] private AnimationCurve _turnEaseCurve;

    public override void PerformObstacleAnim()
    {
        if(_isActive)
        {
            StartCoroutine(Turn());
        }
        SfxManager.Instance.PlaySFX(8697);
    }

    public override void SetToDefaultState()
    {
        _isActive = _defaultState;
    }
    
    private IEnumerator Turn()
    {
        float timeElapsed = 0f;
        float totalDuration = _turnEaseCurve.keys[_turnEaseCurve.length - 1].time;

        float startRotationY = _turnAnchor.transform.eulerAngles.y;

        ////first calculate the target Y rotation (90 degrees to the left or right)
        float targetRotationY = _turnsLeft ? startRotationY - 90f : startRotationY + 90f;
        ////then we need to normalize the angle to prevent values greater than 360 or less than 0
        if (targetRotationY < 0f)
            targetRotationY += 360f;
        else if (targetRotationY >= 360f)
            targetRotationY -= 360f;

        while (timeElapsed < totalDuration)
        {
            float t = _turnEaseCurve.Evaluate(timeElapsed);

            float newRotationY = Mathf.LerpAngle(startRotationY, targetRotationY, t);

            _turnAnchor.transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotationY, transform.eulerAngles.z);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        _turnAnchor.transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetRotationY, transform.eulerAngles.z);
    }
}
