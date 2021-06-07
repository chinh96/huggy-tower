using System.Collections.Generic;
using UnityEngine;

public class Config : ScriptableObject
{
    private static Config instance;
    public static Config Instance => instance ? instance : (instance = Resources.Load<Config>(Constants.CONFIG));

    public int MaxLevelCanReach;
    public int MaxLevelWithOutTutorial;
    public List<int> LevelSkips;
    public int PercentProgressGiftBonused;
    public int CoinBonusPerLevel;
    public bool EnableAds;
    public bool EnableTest;
}