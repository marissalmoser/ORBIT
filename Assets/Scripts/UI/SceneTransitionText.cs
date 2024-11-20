using System.Collections;
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
