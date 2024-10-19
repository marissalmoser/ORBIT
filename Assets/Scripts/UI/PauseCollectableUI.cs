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
    void OnEnable()
    {
        if(!CollectibleManager.Instance.HasCollectable())
        {
            transform.parent.gameObject.SetActive(false);
            return;
        }

        if(CollectibleManager.Instance.GetIsCollected())
        {
            GetComponent<Image>().enabled = true;
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }
}
