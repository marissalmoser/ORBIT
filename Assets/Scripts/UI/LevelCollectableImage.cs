/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Date Created: 10/19/24
 *    Description: Contains functionality for the level and planet buttons to set 
 *      their locked/collectible status.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] int _uniqueLevelIndex;
    [SerializeField] int _planetID;
    [SerializeField] GameObject _newIcon;
    private static int NEW_WORLD = 1;

    private void Awake()
    {
        //new icon functionality
        if (_isPlanet && CollectibleManager.Instance.collectibleStats[_uniqueLevelIndex].GetIsCompleted()
            && _planetID >= NEW_WORLD)
        {
            NEW_WORLD = _planetID;
        }
    }

    private void Start()
    {
        _thisLevel = CollectibleManager.Instance.collectibleStats[_buildIndex];

        //if this is a planet and build index is completed, switch sprite and unlock
         if (_isPlanet && CollectibleManager.Instance.collectibleStats[_uniqueLevelIndex].GetIsCompleted())
         {
             //change to unlocked art
             //TODO
             _thisLevel.SetIsLocked(false);
        
             if(NEW_WORLD == _planetID)
             {
                 //if world 5 is completed, don't enable any new world icons
                 if (_planetID == 5 && CollectibleManager.Instance.collectibleStats[32].GetIsCompleted())
                 {
                     return;
                 }
                 //enable the new image on child
                 _newIcon.SetActive(true);
             }
         }
        //if the level is unlocked, switch button sprite
        if (!_thisLevel.GetIsLocked() && !_isPlanet)
        {
            GetComponent<Image>().sprite = _UnlockedButtonSprite;

            //if the level has a collectable, enable the has a collectable asset
            if (_thisLevel.HasCollectible())
            {
                _collectableAsset.SetActive(true);

                //if the level's collectable has been collected, anable the colleted asset
                if (_thisLevel.GetIsCollected())
                {
                    _collectableCollectedAsset.SetActive(true);
                }
            }
        }
    }

    public void OnMouseUpAsButton()
    {
        SceneTransitionManager.Instance.LoadNewScene(_buildIndex);
    }
}
