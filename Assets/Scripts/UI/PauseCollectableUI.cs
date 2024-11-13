/******************************************************************
 *    Author: Marissa Moser
 *    Contributors:
 *    Date Created: 10/18/24
 *    Description: A helper script for the Collectable Pause menu UI.
 *      Its sets the UI visibility each time it is turned on
 *    
 *******************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseCollectableUI : MonoBehaviour
{
    //when ever the pasuse menu is toggled on, set the collectable UI
    void OnEnable()
    {
        //hides the empty collectable asset of the level doesn't have a collectable.
        if(!CollectibleManager.Instance.HasCollectable())
        {
            transform.parent.gameObject.SetActive(false);
            return;
        }

        //sets the collected asset 
        if(GameManager.Instance.GetCollectableStatus())
        {
            GetComponent<Image>().enabled = true;
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }
}
