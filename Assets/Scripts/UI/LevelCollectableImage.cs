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
        if (CollectibleManager.Instance.collectibleStats[_buildIndex].HasCollectible())
        {
            _collectableAsset.SetActive(true);
        }
        if (CollectibleManager.Instance.collectibleStats[_buildIndex].GetIsCollected())
        {
            _collectableCollectedAsset.SetActive(true);
        }
    }
}
