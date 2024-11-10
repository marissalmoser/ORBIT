/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: September 20, 2024
*    Description: The behavior of the icon that plays when the action order is playing.
*       It automatically clears when the game manager's play action order action is
*       called, and subscribing to the OnNewActionPlayed action will enable the icon
*       and move it, and When the player is done moving, ActionOrderComplete should
*       be invoked to hide the icon again.
*******************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ActionOrderDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform _canvas;
    public static Action NewActionPlayed;
    public static Action ActionOrderComplete;
    public static Action ResetIndicator;

    [SerializeField] private float _moveSpeed;

    RectTransform _rt;
    Image _image;

    private void Start()
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _rt.anchoredPosition = new Vector2(-GameManager.Instance.GetPlayedCards().Count * 120 + UIManager.Instance.cardWidth / 2 - 93, -130);
    }

    private void OnEnable()
    {
        NewActionPlayed += OnNewActionPlayed;
        ActionOrderComplete += ClearIcon;
        ResetIndicator += ResetIndicatorPosition;
        GameManager.PlayActionOrder += SetActionOrder;
        GameManager.PlayDemoActionOrder += SetDemoActionOrder;
    }
    private void OnDisable()
    {
        NewActionPlayed -= OnNewActionPlayed;
        ActionOrderComplete -= ClearIcon;
        GameManager.PlayActionOrder -= SetActionOrder;
        GameManager.PlayDemoActionOrder -= SetDemoActionOrder;
    }

    private void SetActionOrder(List<Card> _input)
    {
        ClearIcon();
    }
    private void SetDemoActionOrder(List<Card> _input)
    {
        ClearIcon();
    }

    /// <summary>
    /// When a new action is played, moves the icon acordingly
    /// </summary>
    private void OnNewActionPlayed()
    {
        if(_image.enabled)
        {
            StartCoroutine(MoveDot());
            return;
        }
        _image.enabled = true;
    }

    /// <summary>
    /// Moves the dot indicator down to the next position
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveDot()
    {
        float timeElapsed = 0f;
        float totalTime = _moveSpeed;
        Vector3 originalPos = _rt.anchoredPosition;
        Vector3 targetPos = new Vector3(_rt.anchoredPosition.x + 120, _rt.anchoredPosition.y);
        while (timeElapsed < totalTime)
        {
            float time = timeElapsed / totalTime;
            _rt.anchoredPosition = Vector3.Lerp(originalPos, targetPos, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _rt.anchoredPosition = targetPos;
    }

    /// <summary>
    /// Resets the icon to its original position and disables the image.
    /// </summary>
    private void ClearIcon()
    {
        _image.enabled = false;
        if (GameManager.Instance.isConfirmCardThere)
            _rt.anchoredPosition = new Vector2((-UIManager.Instance.shiftIndex - 1) * 120 + UIManager.Instance.cardWidth / 2 - 93, -130);
        else
            _rt.anchoredPosition = new Vector2((-UIManager.Instance.shiftIndex - 1) * 120 + UIManager.Instance.cardWidth / 2 - 93, -130);
    }

    private void ResetIndicatorPosition()
    {
        if (_rt != null)
        {
            if (GameManager.Instance.isConfirmCardThere)
                _rt.anchoredPosition = new Vector2((-UIManager.Instance.shiftIndex - 1) * 120 + UIManager.Instance.cardWidth / 2 - 93, -130);
            else
                _rt.anchoredPosition = new Vector2((-UIManager.Instance.shiftIndex - 1) * 120 + UIManager.Instance.cardWidth / 2 - 93, -130);
        }
    }
}
