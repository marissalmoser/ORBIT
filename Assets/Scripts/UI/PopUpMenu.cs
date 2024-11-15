/******************************************************************
 *    Author: Sky Turner 
 *    Contributors: 
 *    Date Created: 9/21/24
 *    Description: This handles the Pop Up menu logic 
 *    
 *******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMenu : MonoBehaviour
{
    //[SerializeField] private float _secondsToFade;
    //[SerializeField] private GameObject _popUpMenu;
    //private Animator _animator; 

    //private void Start()
    //{
    //    _animator = GetComponent<Animator>();
    //    StartCoroutine(StartFadeTimer());
    //}

    /// <summary>
    /// A timer that controls when the pop up menu starts fading
    /// </summary>
    /// <returns></returns>
    //private IEnumerator StartFadeTimer()
    //{
    //    float timer = _secondsToFade;

    //    while (timer > 0)
    //    {
    //        timer -= Time.deltaTime;
    //        yield return null;
    //    }

    //    StartFade();
    //}

    /// <summary>
    /// Sets an animator trigger 
    /// </summary>
    //private void StartFade()
    //{
    //    _animator.SetTrigger("FadeOut");
    //}

    /// <summary>
    /// Sets the pop up menu gameobject to inactive, skipping the popup
    /// </summary>
    public void SkipPopUp()
    {
        PlayerController.StartPlayerFall?.Invoke();
        gameObject.SetActive(false);
    }
}
