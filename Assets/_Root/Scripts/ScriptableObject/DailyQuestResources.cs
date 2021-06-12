using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DailyQuestResources", menuName = "ScriptableObjects/DailyQuestResources")]
public class DailyQuestResources : ScriptableObject
{
    public List<DailyQuestDay> DailyQuestDays;

    public DailyQuestDay DailyQuestDayCurrent => DailyQuestDays[Data.TotalDays % DailyQuestDays.Count];

    public void IncreaseByType(DailyQuestType type, int value = 1)
    {
        var data = GetDataByType(type);
        data.NumberCurrent += value;

        if (data.HasNoti)
        {
            NotiQuestController.Instance.Show(data);
        }
    }

    public DailyQuestData GetDataByType(DailyQuestType type)
    {
        return DailyQuestDayCurrent.DailyQuestDatas.Find(item => item.Type == type);
    }

    public bool HasNoti
    {
        get
        {
            foreach (var DailyQuest in DailyQuestDayCurrent.DailyQuestDatas)
            {
                if (DailyQuest.HasNoti)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public void Reset()
    {
        DailyQuestDays[(Data.TotalDays - 1) % DailyQuestDays.Count].DailyQuestDatas.ForEach(item =>
        {
            item.NumberCurrent = 0;
            item.IsClaimed = false;
        });
    }
}

[Serializable]
public class DailyQuestDay
{
    public List<DailyQuestData> DailyQuestDatas;
}

[Serializable]
public class DailyQuestData
{
    public DailyQuestType Type;
    [GUID] public string Id;
    public string Text;
    public Sprite Sprite;
    public int NumberTarget;
    public int Bonus;
    public string Number => (NumberCurrent > NumberTarget ? NumberTarget : NumberCurrent) + "/" + NumberTarget;
    public string Title => Text.Replace("{}", NumberTarget.ToString());
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
