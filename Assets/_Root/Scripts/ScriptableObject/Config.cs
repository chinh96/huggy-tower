using System.Collections.Generic;
using UnityEngine;

public class Config : ScriptableObject
{
    public int MaxLevelCanReach;
    public int MaxLevelWithOutTutorial;
    public List<int> LevelSkips;
    public int PercentProgressGiftBonused;
    public int CoinBonusPerLevel;
    public bool EnableAds;
    public bool EnableTest;
}