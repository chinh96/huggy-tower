using System;
using System.Collections.Generic;
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
    public List<SkinData> SkinsLocked => SkinDatas.FindAll(item => !item.IsUnlocked);
    public List<SkinData> SkinsCoin => SkinDatas.FindAll(item => !item.IsUnlocked && item.SkinType == SkinType.Coin);
    public SkinData SkinAchievement => SkinDatas.Find(item => item.SkinType == SkinType.Achievement);
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
}



[Serializable]
public class SkinData
{
    [SpineSkin] public string SkinName;

    [GUID] public string Id;

    public SkinType SkinType;

    public int Coin;

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