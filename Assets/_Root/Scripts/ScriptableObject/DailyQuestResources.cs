using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DailyQuestResources", menuName = "ScriptableObjects/DailyQuestResources")]
public class DailyQuestResources : ScriptableObject
{
    public List<DailyQuestData> DailyQuestDatas;

    public List<DailyQuestDay> DailyQuestDays;

    public DailyQuestDay DailyQuestDayCurrent => DailyQuestDays[Data.TotalDays % DailyQuestDays.Count];

    public void IncreaseByType(DailyQuestType type, int value = 1)
    {
        var item = GetItemByType(type);

        if (NotiQuestController.Instance != null && item != null)
        {
            item.NumberCurrent += value;

            if (item.HasNoti)
            {
                NotiQuestController.Instance.Show(item);
            }
        }
    }

    public DailyQuestDayItem GetItemByType(DailyQuestType type)
    {
        return DailyQuestDayCurrent.DailyQuestDayItems.Find(item => item.Type == type);
    }

    public bool HasNoti
    {
        get
        {
            foreach (var item in DailyQuestDayCurrent.DailyQuestDayItems)
            {
                if (item.HasNoti)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public void Reset()
    {
        int totalDays = Data.TotalDays;
        if (totalDays > 0)
        {
            DailyQuestDays[(Data.TotalDays - 1) % DailyQuestDays.Count].DailyQuestDayItems.ForEach(item =>
            {
                item.NumberCurrent = 0;
                item.IsClaimed = false;
            });
        }

        DailyQuestDayCurrent.DailyQuestDayItems.ForEach(item =>
        {
            item.dailyQuestData = DailyQuestDatas.Find(data => data.Type == item.Type);
            Debug.Log(item.dailyQuestData);
        });
    }
}

[Serializable]
public class DailyQuestDay
{
    public List<DailyQuestDayItem> DailyQuestDayItems;
}

[Serializable]
public class DailyQuestDayItem
{
    [NonSerialized] public DailyQuestData dailyQuestData;
    public DailyQuestType Type;
    [GUID] public string Id;
    public int NumberTarget;
    public int Bonus;
    public Sprite Sprite => dailyQuestData.Sprite;
    public string Number => (NumberCurrent > NumberTarget ? NumberTarget : NumberCurrent) + "/" + NumberTarget;
    public string Title => dailyQuestData.Text.Replace("{}", NumberTarget.ToString());
    public int NumberCurrent
    {
        get { Data.DailyQuestId = Id; return Data.DailyQuestNumberCurrent; }

        set { Data.DailyQuestId = Id; Data.DailyQuestNumberCurrent = value; }
    }
    public bool IsUnlocked => NumberCurrent >= NumberTarget;
    public bool IsClaimed
    {
        get { Data.IdCheckUnlocked = Id + "Claimed"; return Data.IsUnlocked; }

        set { Data.IdCheckUnlocked = Id + "Claimed"; Data.IsUnlocked = value; }
    }
    public bool HasNoti => IsUnlocked && !IsClaimed;
}

[Serializable]
public class DailyQuestData
{
    public DailyQuestType Type;
    public string Text;
    public Sprite Sprite;
}
