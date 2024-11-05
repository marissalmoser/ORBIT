/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 10/19/24
 *    Description: Contains functionality for the level and planet buttons to set 
 *      their locked/collectible status.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCollectableImage : MonoBehaviour
{
    [SerializeField] int _buildIndex;
    [SerializeField] GameObject _collectableAsset;
    [SerializeField] GameObject _collectableCollectedAsset;
    [SerializeField] Sprite _UnlockedButtonSprite;

    private CollectibleStats _thisLevel;

    [SerializeField] bool _isPlanet;
    [SerializeField] int _planetID;
    private static int NEW_WORLD = 1;

    private void Awake()
    {
        //new icon functionality
        if (_isPlanet && CollectibleManager.Instance.collectibleStats[_buildIndex].GetIsCompleted()
            && _planetID >= NEW_WORLD)
        {
            NEW_WORLD = _planetID;
        }
    }

    private void Start()
    {
        _thisLevel = CollectibleManager.Instance.collectibleStats[_buildIndex];

        //if this is a planet and build index is completed, switch sprite and unlock
        if (_isPlanet && _thisLevel.GetIsCompleted())
        {
            GetComponent<Image>().sprite = _UnlockedButtonSprite;
            CollectibleManager.Instance.collectibleStats[_buildIndex].SetIsLocked(false);

            if(NEW_WORLD == _planetID)
            {
                //enable the new image on child
                Image[] children = GetComponentsInChildren<Image>();
                for(int i = 0; i < children.Length; i++)
                {
                    children[i].enabled = true;
                }
            }
        }
        //if the level is unlocked, switch button sprite
        else if (!_thisLevel.GetIsLocked() && !_isPlanet)
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
