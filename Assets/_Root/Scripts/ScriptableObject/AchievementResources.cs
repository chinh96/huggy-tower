using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AchievementResources", menuName = "ScriptableObjects/AchievementResources")]
public class AchievementResources : ScriptableObject
{
    public List<AchievementData> AchievementDatas;
    public List<AchievementTargetData> AchievementTargetDatas;

    private void Start()
    {
        ResetNumberTemp();
    }

    public void IncreaseByType(AchievementType type, int value = 1)
    {
        var data = GetDataByType(type);
        if (data != null)
        {
            CheckNumber(type, value, data);
        }
    }

    private void CheckNumber(AchievementType type, int value, AchievementData data)
    {
        switch (type)
        {
            case AchievementType.PlayToLevel:
                data.NumberCurrent = Data.CurrentLevel;
                break;
            case AchievementType.BuySkin:
                data.NumberCurrent = ResourcesController.Hero.SkinsIsUnlocked.Count;
                break;
            default:
                if (GameController.Instance == null)
                {
                    data.NumberCurrent += value;
                }
                else
                {
                    if (data.NumberTemp == 0)
                    {
                        data.NumberTemp = data.NumberCurrent + value;
                    }
                    else
                    {
                        data.NumberTemp += value;
                    }
                }
                break;
        }
    }

    public void ResetNumberTemp()
    {
        AchievementDatas.ForEach(data =>
        {
            data.NumberTemp = 0;
        });
    }

    public void UpdateNumberCurrent()
    {
        AchievementDatas.ForEach(data =>
        {
            if (data.NumberTemp > 0)
            {
                data.NumberCurrent = data.NumberTemp;
            }
        });

        ResetNumberTemp();
    }

    public AchievementData GetDataByType(AchievementType type)
    {
        return AchievementDatas.Find(item => item.Type == type);
    }

    public List<AchievementData> GetDatasIsClaimed()
    {
        return AchievementDatas.FindAll(item => item.IsClaimed);
    }

    public bool HasNoti
    {
        get
        {
            foreach (var achievement in AchievementDatas)
            {
                if (achievement.NumberCurrent >= achievement.NumberTarget)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

[Serializable]
public class AchievementData
{
    public AchievementType Type;
    [GUID] public string Id;
    public string Text;
    public Sprite Sprite;
    public int NumberTarget;
    public int Bonus;
    public string Number => (NumberCurrent < NumberTarget ? NumberCurrent : NumberTarget) + "/" + NumberTarget;
    public string Title => Text.Replace("{}", NumberTarget.ToString());
    public int NumberTemp;
    public int NumberCurrent
    {
        get { Data.AchievementId = Id; return Data.AchievementNumberCurrent; }

        set { Data.AchievementId = Id; Data.AchievementNumberCurrent = value; }
    }
    public bool IsClaimed
    {
        get { Data.IdCheckUnlocked = Id + "Claimed"; return Data.IsUnlocked; }

        set { Data.IdCheckUnlocked = Id + "Claimed"; Data.IsUnlocked = value; }
    }
}

[Serializable]
public class AchievementTargetData
{
    public int Number;
    [GUID] public string Id;
    public bool IsClaimed
    {
        get { Data.IdCheckUnlocked = Id + "Claimed"; return Data.IsUnlocked; }

        set { Data.IdCheckUnlocked = Id + "Claimed"; Data.IsUnlocked = value; }
    }
}
