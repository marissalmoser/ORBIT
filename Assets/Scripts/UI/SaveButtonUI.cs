/******************************************************************
*    Author: Elijah Vroman
*    Contributors: Elijah Vroman
*    Date Created: 10/28/24
*    Description: Got tired of trying to find a workaround to put 
*    into UIButtons or SLM. 
*******************************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite Icon1;
    [SerializeField] private Sprite Icon2;
    [SerializeField] private Sprite Icon3;

    public void OnEnable()
    {
        Invoke("ChangeSaveImage", 0.5f);
    }
    public void ChangeSaveImage()
    {
        if (SaveLoadManager.Instance.GetCurrentSaveSelected().ToString() == "1")
        {
            image.sprite = Icon1;
        }
        else if (SaveLoadManager.Instance.GetCurrentSaveSelected().ToString() == "2")
        {
            image.sprite = Icon2;
        }
        else if (SaveLoadManager.Instance.GetCurrentSaveSelected().ToString() == "3")
        {
            image.sprite = Icon3;
        }
    }
}
