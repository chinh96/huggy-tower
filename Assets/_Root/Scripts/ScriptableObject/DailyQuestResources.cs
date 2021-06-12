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

    public void IncreaseByCondition(ELevelCondition condition)
    {
        GetDailyQuestByCondition(condition).NumberCurrent++;
    }

    public bool IsUnlockedByCondition(ELevelCondition condition)
    {
        return GetDailyQuestByCondition(condition).IsUnlocked;
    }

    public bool IsUnlockAll()
    {
        return DailyQuestDatasCurrent.TrueForAll(item => item.IsUnlocked);
    }

    public void Reset()
    {
        DailyQuestItems[(Data.TotalDays - 1) % DailyQuestItems.Count].DailyQuestDatas.ForEach(item =>
        {
            item.NumberCurrent = 0;
            item.IsClaimed = false;
        });
    }

    public bool HasNoti
    {
        get
        {
            foreach (DailyQuestData dailyQuestData in DailyQuestDatasCurrent)
            {
                if (dailyQuestData.IsUnlocked && !dailyQuestData.IsClaimed)
                {
                    return true;
                }
            }

            return false;
        }
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
    public int NumberTarget;
    public Sprite Sprite => ResourcesController.Quest.GetQuestByCondition(Condition).SpriteSquare;
    public string Title => ResourcesController.Quest.GetQuestByCondition(Condition).Quest;
    public int NumberCurrent
    {
        get
        {
            Data.DailyQuestId = Id;
            return Data.DailyQuestNumberCurrent;
        }

        set
        {
            Data.DailyQuestId = Id;
            Data.DailyQuestNumberCurrent = value;
        }
    }
    public bool IsUnlocked => NumberCurrent >= NumberTarget;
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
