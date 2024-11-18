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
    [SerializeField] private int _challengeLevelIndex;
    [SerializeField] private int _uniqueLevelIndex;
    [SerializeField] private TextMeshProUGUI _counter;
    [SerializeField] private Image _collectableDisplay;
    [SerializeField] private GameObject _challengeButton;

    [SerializeField] private Sprite _half;

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

        //set collectable text
        _counter.text = levelsCollected + "/" + levelsWithCollectable;

        //set collectable display
        if(levelsCollected >= 1)
        {
            _collectableDisplay.sprite = _half;
        }
        if (levelsCollected >= 3)
        {
            //disable image, enable button
            _counter.gameObject.SetActive(false);
            _collectableDisplay.enabled = false;
            _challengeButton.SetActive(true);

            //play animation if challenge level has not been beat
            if (!_cm.collectibleStats[_challengeLevelIndex].GetIsCompleted())
            {
                GetComponent<Animator>().enabled = true;
            }
            
            //unlock collectable manager
            _cm.collectibleStats[_challengeLevelIndex].SetIsLocked(false);
        }
    }
}
