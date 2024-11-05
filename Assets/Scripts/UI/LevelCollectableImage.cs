/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 10/19/24
 *    Description: Contains functionality for the collectable images 
 *      per level on the collectable manager.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCollectableImage : MonoBehaviour
{
    [SerializeField] int _buildIndex;
    [SerializeField] GameObject _collectableAsset;
    [SerializeField] GameObject _collectableCollectedAsset;
    [SerializeField] Sprite _UnlockedButtonSprite;

    private CollectibleStats _thisLevel;

    private void Start()
    {
        _thisLevel = CollectibleManager.Instance.collectibleStats[_buildIndex];

        //if the level is unlocked, switch button sprite
        if (!_thisLevel.GetIsLocked())
        {
            GetComponent<Image>().sprite = _UnlockedButtonSprite;

            //if the level has a collectable, enable the has a collectable asset
            if (_thisLevel.HasCollectible())
            {
                _collectableAsset.SetActive(true);
            }

            //if the level's collectable has been collected, anable the colleted asset
            if (_thisLevel.GetIsCollected())
            {
                _collectableCollectedAsset.SetActive(true);
            }
        }
    }
}
