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

    public void Reset()
    {
        //int number = ResourcesController.Achievement.GetDatasIsClaimed().Count;
        //ResourcesController.Hero.SkinAchievements.ForEach(skin => skin.IsUnlocked = number >= skin.NumberAchievement);
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
            case AchievementType.CompleteEarth:
            case AchievementType.CompleteDesert:
            case AchievementType.CompleteIceland:
            case AchievementType.CompleteInferno:
            case AchievementType.CompleteJade:
            case AchievementType.CompleteOlympus:
            case AchievementType.JoinGroupFacebookSuccessfully:
            case AchievementType.ClaimDailyReward:
                data.NumberCurrent += value;
                break;
            case AchievementType.PlayToLevel:
                data.NumberCurrent = Data.CurrentLevel;
                break;
            case AchievementType.BuySkin:
                data.NumberCurrent = ResourcesController.Hero.SkinsIsUnlocked.Count;
                break;
            default:
                if (data.NumberTemp == 0)
                {
                    data.NumberTemp = data.NumberCurrent + value;
                }
                else
                {
                    data.NumberTemp += value;
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
                if (achievement.HasNoti)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public void CheckCompleteCastle()
    {
        //ResourcesController.Universe.Worlds.ForEach(world =>
        //{
        //    if (world.IsComplete)
        //    {
        //        switch (world.WorldType)
        //        {
        //            case WorldType.Earth:
        //                IncreaseByType(AchievementType.CompleteEarth);
        //                break;
        //            case WorldType.Desert:
        //                IncreaseByType(AchievementType.CompleteDesert);
        //                break;
        //            case WorldType.Iceland:
        //                IncreaseByType(AchievementType.CompleteIceland);
        //                break;
        //            case WorldType.Inferno:
        //                IncreaseByType(AchievementType.CompleteInferno);
        //                break;
        //            case WorldType.Jade:
        //                IncreaseByType(AchievementType.CompleteJade);
        //                break;
        //            case WorldType.Olympus:
        //                IncreaseByType(AchievementType.CompleteOlympus);
        //                break;
        //        }
        //    }
        //});
    }

    public string ConvertData()
    {
        AchievementDataModel[] models = new AchievementDataModel[AchievementDatas.Count];

        for (int i = 0; i < AchievementDatas.Count; i++)
        {
            models[i] = new AchievementDataModel() { isClaimed = AchievementDatas[i].IsClaimed, number = AchievementDatas[i].NumberCurrent };
        }

        return JsonHelper.ToJson(models);
    }

    public string ConvertDataTarget()
    {
        DataModel[] models = new DataModel[AchievementTargetDatas.Count];

        for (int i = 0; i < AchievementTargetDatas.Count; i++)
        {
            models[i] = new DataModel() { data = AchievementTargetDatas[i].IsClaimed.ToString() };
        }

        return JsonHelper.ToJson(models);
    }

    public void TransformData(string raw)
    {
        var result = JsonHelper.FromJson<AchievementDataModel>(raw);

        int count = result.Length;
        if (count > AchievementDatas.Count) count = AchievementDatas.Count;

        for (int i = 0; i < count; i++)
        {
            AchievementDatas[i].IsClaimed = result[i].isClaimed;
            AchievementDatas[i].NumberCurrent = result[i].number;
        }
    }

    public void TransformTargetData(string raw)
    {
        var result = JsonHelper.FromJson<DataModel>(raw);

        int count = result.Length;
        if (count > AchievementTargetDatas.Count) count = AchievementTargetDatas.Count;

        for (int i = 0; i < count; i++)
        {
            AchievementTargetDatas[i].IsClaimed = bool.Parse(result[i].data.ToLower());
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
    public bool HasNoti => IsUnlocked && !IsClaimed;
    public bool IsUnlocked => NumberCurrent >= NumberTarget;
    public bool IsClaimed
    {
        get { Data.IdCheckUnlocked = Id + "Claimed"; return Data.IsUnlocked; }

        set { Data.IdCheckUnlocked = Id + "Claimed"; Data.IsUnlocked = value; }
    }
    public int NumberCurrent
    {
        get { Data.AchievementId = Id; return Data.AchievementNumberCurrent; }

        set { Data.AchievementId = Id; Data.AchievementNumberCurrent = value; }
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

[Serializable]
public class AchievementDataModel
{
    public bool isClaimed;
    public int number;
}
