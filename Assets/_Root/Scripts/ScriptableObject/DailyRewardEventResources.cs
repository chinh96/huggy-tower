using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DailyRewardEventResources", menuName = "ScriptableObjects/DailyRewardEventResources")]
public class DailyRewardEventResources : ScriptableObject
{
    // Start is called before the first frame update
    public List<ItemConfigEvent> DailyRewards;

    public List<ItemConfigCollectEvent> EventCollectRewards;

    // public List<int> DailyRewardsLoop;

    // public List<int> DailyRewardsSkin;

    // public bool HasNoti => Data.DailyRewardCurrent <= Data.TotalDays;
}

[Serializable]
public class ItemConfigEvent
{
    public int Coin;
    public int CandyXmas;
    public int SkinId;

}
[Serializable]
public class ItemConfigCollectEvent
{

    [GUID] public string Id;
    public int NumCandyXmas;
    public int SkinId;
    public bool isSkinPrincess;

}


