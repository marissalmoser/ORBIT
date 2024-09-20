/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 20, 2024
*    Description: 
*******************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;

public class ActionOrderDisplay : MonoBehaviour
{
    public static Action ShakeCard;
    private List<Card> _currentActionOrder;
    private int _indexToShake = 0;

    private void OnEnable()
    {
        ShakeCard += OnShake;
        GameManager.PlayActionOrder += SetActionOrder;
        GameManager.PlayDemoActionOrder += SetDemoActionOrder;
    }
    private void OnDisable()
    {
        ShakeCard -= OnShake;
        GameManager.PlayActionOrder -= SetActionOrder;
        GameManager.PlayDemoActionOrder -= SetDemoActionOrder;
    }

    private void SetActionOrder(List<Card> _input)
    {
        _currentActionOrder = _input;
        _indexToShake = 0;
    }

    private void SetDemoActionOrder(List<Card> _input)
    {
        _currentActionOrder = _input;
        _indexToShake = 0;
    }

    private void OnShake()
    {
        //stop all current shaking

        //shake card at index

        //asign next card
        if (_currentActionOrder[_indexToShake + 1] != null)
        {
            _indexToShake++;
        }
    }
}
