using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DailyQuestResources", menuName = "ScriptableObjects/DailyQuestResources")]
public class DailyQuestResources : ScriptableObject
{
    public List<DailyQuestData> DailyQuestDatas;

    public int TotalDays => (int)(DateTime.Now - DateTime.Parse(Data.DateTimeStart)).TotalDays;
}

[Serializable]
public class DailyQuestData
{
    public List<ELevelCondition> Quests;
}
