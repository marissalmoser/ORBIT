/******************************************************************
*    Author: Elijah Vroman
*    Contributors: Elijah Vroman
*    Date Created: 10/28/24
*    Description: Got tired of trying to find a workaround to put 
*    into UIButtons or SLM. 
*******************************************************************/
using TMPro;
using UnityEngine;

public class SaveButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Start()
    {
        text.text = SaveLoadManager.Instance.GetCurrentSaveSelected().ToString();
    }
}
