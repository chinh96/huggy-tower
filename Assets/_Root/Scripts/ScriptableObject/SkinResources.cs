using System;
using System.Collections.Generic;
using System.Text;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SkinResources", menuName = "ScriptableObjects/SkinResources")]
public class SkinResources : ScriptableObject, IHasSkeletonDataAsset
{
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    public List<SkinData> SkinDatas;
    [SerializeField, SpineSkin] private string skinNameDefault;

    public SkinData SkinDefault => SkinDatas.Find(item => item.SkinName == skinNameDefault);
    public List<SkinData> SkinsDailyReward => SkinDatas.FindAll(item => item.SkinType == SkinType.Daily);
    public List<SkinData> SkinsCoin => SkinDatas.FindAll(item => !item.IsUnlocked && item.SkinType == SkinType.Coin);
    public List<SkinData> SkinAchievements => SkinDatas.FindAll(item => item.SkinType == SkinType.Achievement);
    public SkinData SkinGiftcode => SkinDatas.Find(item => item.SkinType == SkinType.Giftcode);
    public List<SkinData> SkinsIsUnlocked => SkinDatas.FindAll(item => item.IsUnlocked && item.SkinName != skinNameDefault);

    public int TotalSkinUnlocked()
    {
        int count = 0;
        for (int i = 0; i < SkinDatas.Count; i++)
        {
            if (SkinDatas[i].IsUnlocked)
            {
                count++;
            }
        }

        return count;
    }
    public bool HasNoti
    {
        get
        {
            foreach (var skin in SkinsCoin)
            {
                if (Data.CoinTotal >= skin.Coin)
                {
                    return true;
                }
            }

            return false;
        }
    }
    public void Reset()
    {
        SkinDefault.IsUnlocked = true;
    }

    public string ConvertData()
    {
        StringBuilder result = new StringBuilder("");
        for (int i = 0; i < SkinDatas.Count; i++)
        {
            result.Append($"{SkinDatas[i].IsUnlocked}@");
        }

        result.Remove(result.Length - 1, 1);
        return result.ToString();
    }

    public void TransformTargetData(string raw)
    {
        var result = raw.Split('@');

        int count = result.Length;
        if (count > SkinDatas.Count) count = SkinDatas.Count;

        for (int i = 0; i < count; i++)
        {
            SkinDatas[i].IsUnlocked = bool.Parse(result[i].ToLower());
        }
    }
}



[Serializable]
public class SkinData
{
    [SpineSkin] public string SkinName;

    [GUID] public string Id;

    public string Name;

    public SkinType SkinType;
    public RescuePartyType RescuePartyType;

    public int Coin;

    public string Giftcode;
    public int NumberMedalTarget;
    public int DayDaily;
    public int NumberAchievement;
    public bool HasNotiRescueParty
    {
        get
        {
            if (!IsUnlocked)
            {
                if (RescuePartyType == RescuePartyType.Top100)
                {
                    return false;
                }
                else
                {
                    return Data.TotalGoldMedal >= NumberMedalTarget;
                }
            }
            else
            {
                return false;
            }
        }
    }

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
}