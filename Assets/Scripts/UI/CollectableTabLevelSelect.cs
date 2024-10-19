/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 10/19/24
 *    Description: Contains functionality for the ratio of collectables
 *      collected per world in the level select.
 *******************************************************************/
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

        // loop thru levels in this world and set ints for ratio
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
