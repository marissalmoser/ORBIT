using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectableTabLevelSelect : MonoBehaviour
{
    [SerializeField] private List<int> _levelsInWorld = new List<int>();
    [SerializeField] private TextMeshProUGUI _counter;

    private CollectibleManager _cm;

    private void Start()
    {
        int levelsWithCollectable = 0;
        int levelsCollected = 0;
        _cm = CollectibleManager.Instance;

        //set ints
        foreach (int level in _levelsInWorld )
        {
            if (_cm.collectibleStats[level].HasCollectible())
            {
                levelsWithCollectable++;
            }
            if (_cm.collectibleStats[level].GetIsCollected())
            {
                levelsCollected++;
            }
        }

        //set text
        _counter.text = levelsCollected + "/" + levelsWithCollectable;
    }
}
