/******************************************************************
 *    Author: Marissa Moser
 *    Contributors:
 *    Date Created: 10/18/24
 *    Description: A helper script for the Collectable Pause menu UI.
 *      Its sets the UI visibility each time it is turned on as well as the
 *      level name text.
 *    
 *******************************************************************/
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseCollectableUI : MonoBehaviour
{
    [SerializeField] private bool _isImage;
    //when ever the pasuse menu is toggled on, set the collectable UI
    void OnEnable()
    {
        if (_isImage)
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
        else
        {
            GetComponent<TextMeshProUGUI>().text = CollectibleManager.Instance.GetCurrentLevelName();
        }
    }
}
