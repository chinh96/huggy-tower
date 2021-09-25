using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigResources", menuName = "ScriptableObjects/ConfigResources")]
public class ConfigResources : ScriptableObject
{
    public LevelMap LevelDebug;
    public int MaxLevelCanReach;
    public int MaxLevelWithOutTutorial;
    public List<int> LevelSkips;
    public int PercentProgressGiftBonused;
    public int CoinBonusPerLevel;
    public bool EnableAds;
    public bool EnableTest;
    public int LevelShowRate;
    public FighterOverlay FighterOverlay;
}