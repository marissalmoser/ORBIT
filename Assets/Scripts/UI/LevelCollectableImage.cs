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

    private void Start()
    {
        //if the level has a collectable, enable the has a collectable asset
        if (CollectibleManager.Instance.collectibleStats[_buildIndex].HasCollectible())
        {
            _collectableAsset.SetActive(true);
        }

        //if the level's collectable has been collected, anable the colleted asset
        if (CollectibleManager.Instance.collectibleStats[_buildIndex].GetIsCollected())
        {
            _collectableCollectedAsset.SetActive(true);
        }
    }
}
