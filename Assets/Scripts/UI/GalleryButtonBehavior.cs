using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryButtonBehavior : MonoBehaviour
{
    [SerializeField] private List<int> _challengeLevels = new List<int>();
    [SerializeField] private GameObject _button;
    void Start()
    {
        //loop thru the challenge levels and check if they are completed. If one is
        //not complete, return
        CollectibleManager cm = CollectibleManager.Instance;
        for(int i = 0;  i < _challengeLevels.Count; i++)
        {
            if (!cm.collectibleStats[i].GetIsCompleted())
            {
                return;
            }
        }

        //if all challenge levels are completed, enable gallery
        _button.SetActive(true);
    }
}
