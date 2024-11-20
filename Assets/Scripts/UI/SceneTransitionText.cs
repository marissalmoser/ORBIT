/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 11/20/24
 *    Description: Sets the text from the list of success messages on the
 *  scene transition scenes on enable.
 *******************************************************************/
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneTransitionText : MonoBehaviour
{
    [SerializeField] private List<string> _succesMessages = new List<string>();
    void OnEnable()
    {
        GetComponent<TextMeshProUGUI>().text = _succesMessages[Random.Range(0, _succesMessages.Count)];
    }
}
