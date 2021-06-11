using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DailyQuestResources", menuName = "ScriptableObjects/DailyQuestResources")]
public class DailyQuestResources : ScriptableObject
{
    public List<DailyQuestItemData> DailyQuestItems;

    public List<DailyQuestData> DailyQuestDatasCurrent => DailyQuestItems[Data.TotalDays % DailyQuestItems.Count].DailyQuestDatas;

    public DailyQuestData GetDailyQuestByCondition(ELevelCondition condition)
    {
        return DailyQuestDatasCurrent.Find(item => item.Condition == condition);
    }

    public void UnlockByCondition(ELevelCondition condition)
    {
        GetDailyQuestByCondition(condition).IsUnlocked = true;
    }

    public bool IsUnlockAll()
    {
        return DailyQuestDatasCurrent.TrueForAll(item => item.IsUnlocked);
    }

    public void Reset()
    {
        DailyQuestItems[(Data.TotalDays - 1) % DailyQuestItems.Count].DailyQuestDatas.ForEach(item =>
        {
            item.IsUnlocked = false;
            item.IsClaimed = false;
        });
    }
}

[Serializable]
public class DailyQuestItemData
{
    public List<DailyQuestData> DailyQuestDatas;
}

[Serializable]
public class DailyQuestData
{
    public ELevelCondition Condition;
    [GUID] public string Id;
    public int Bonus;
    public Sprite Sprite => ResourcesController.Quest.GetQuestByCondition(Condition).SpriteSquare;
    public string Title => ResourcesController.Quest.GetQuestByCondition(Condition).Quest;
    public bool IsUnlocked
    {
        get
        {
            Data.IdCheckUnlocked = Id;
            return Data.IsUnlocked;
        }

        set
        {
            Data.IdCheckUnlocked = Id;
            Data.IsUnlocked = value;
        }
    }
    public bool IsClaimed
    {
        get
        {
            Data.IdCheckUnlocked = Id + "Claimed";
            return Data.IsUnlocked;
        }

        set
        {
            Data.IdCheckUnlocked = Id + "Claimed";
            Data.IsUnlocked = value;
        }
    }
}
