using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AchievementResources", menuName = "ScriptableObjects/AchievementResources")]
public class AchievementResources : ScriptableObject
{
    public List<AchievementData> AchievementDatas;

    public void IncreaseByType(AchievementType type, int value = 1)
    {
        var data = GetDataByType(type);
        data.NumberCurrent += value;
    }

    public AchievementData GetDataByType(AchievementType type)
    {
        return AchievementDatas.Find(item => item.Type == type);
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
    public int NumberCurrent
    {
        get { Data.AchievementId = Id; return Data.AchievementNumberCurrent; }

        set { Data.AchievementId = Id; Data.AchievementNumberCurrent = value; }
    }
    public int NumberTarget
    {
        get { Data.AchievementId = Id; return Data.AchievementNumberTarget; }

        set { Data.AchievementId = Id; Data.AchievementNumberTarget = value; }
    }
}
