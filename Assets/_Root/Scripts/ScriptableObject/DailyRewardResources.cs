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

    public int TotalDays => (int)(DateTime.Now - DateTime.Parse(Data.DateTimeStart)).TotalDays;

    public bool HasNoti => Data.DailyRewardCurrent > TotalDays;
}