using DG.Tweening;
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

    private void Start()
    {
        ResetNumberTemp();
    }

    public void IncreaseByType(DailyQuestType type, int value = 1)
    {
        var item = GetItemByType(type);

        if (item != null)
        {
            CheckNumber(type, value, item);
            CheckNotiQuest(item);
        }
    }

    private void CheckNumber(DailyQuestType type, int value, DailyQuestDayItem item)
    {
        switch (type)
        {
            case DailyQuestType.CompleteEarth:
            case DailyQuestType.CompleteDesert:
            case DailyQuestType.CompleteIceland:
            case DailyQuestType.CompleteInferno:
            case DailyQuestType.CompleteJade:
            case DailyQuestType.CompleteOlympus:
            case DailyQuestType.LogIntoTheGame:
            case DailyQuestType.WatchVideoReward:
                item.NumberCurrent += value;
                break;
            case DailyQuestType.BuySkin:
                item.NumberCurrent = ResourcesController.Hero.SkinsIsUnlocked.Count;
                break;
            default:
                if (item.NumberTemp == 0)
                {
                    item.NumberTemp = item.NumberCurrent + value;
                }
                else
                {
                    item.NumberTemp += value;
                }
                break;
        }
    }

    private void CheckNotiQuest(DailyQuestDayItem item)
    {
        if (NotiQuestController.Instance != null && item.HasNoti && !item.IsShownNoti)
        {
            NotiQuestController.Instance.Save(item);

            DOTween.Sequence().AppendInterval(.1f).AppendCallback(() =>
            {
                if (GameController.Instance.GameState == EGameState.Playing)
                {
                    NotiQuestController.Instance.Show();
                }
            });
        }
    }

    public void ResetNumberTemp()
    {
        DailyQuestDayCurrent.DailyQuestDayItems.ForEach(data =>
        {
            data.NumberTemp = 0;
        });
    }

    public void UpdateNumberCurrent()
    {
        DailyQuestDayCurrent.DailyQuestDayItems.ForEach(data =>
        {
            if (data.NumberTemp > 0)
            {
                data.NumberCurrent = data.NumberTemp;
            }
        });

        ResetNumberTemp();
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
    public int NumberTemp;
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
    public bool IsShownNoti
    {
        get { Data.IdCheckUnlocked = Id + "ShownNoti"; return Data.IsUnlocked; }

        set { Data.IdCheckUnlocked = Id + "ShownNoti"; Data.IsUnlocked = value; }
    }
}

[Serializable]
public class DailyQuestData
{
    public DailyQuestType Type;
    public string Text;
    public Sprite Sprite;
}
