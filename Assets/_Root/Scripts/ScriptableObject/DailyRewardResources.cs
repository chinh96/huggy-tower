using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "DailyRewardResources", menuName = "ScriptableObjects/DailyRewardResources")]
public class DailyRewardResources : ScriptableObject
{
    public List<int> DailyRewards;

    public List<int> DailyRewardsLoop;

    public List<int> DailyRewardsSkin;

    public bool HasNoti => Data.DailyRewardCurrent < Data.TotalDays;
}