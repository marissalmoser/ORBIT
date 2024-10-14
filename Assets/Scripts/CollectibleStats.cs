using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleStats
{
    public string LevelName { get; set; }
    public int BuildIndex { get; set; }
    public bool IsCollected { get; private set; }
    public bool HasCollectible { get; set; }

    public CollectibleStats(string levelName, int buildIndex, bool hasCollectible)
    {
        LevelName = levelName;
        BuildIndex = buildIndex;
        HasCollectible = hasCollectible;
        IsCollected = false;
    }

    public void SetIsCollected(bool collected)
    {
        IsCollected = collected;
    }
}
