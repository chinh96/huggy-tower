using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SkinResources", menuName = "ScriptableObjects/SkinResources")]
public class SkinResources : ScriptableObject, IHasSkeletonDataAsset
{
    private static SkinResources instance;
    public static SkinResources Instance => instance ? instance : instance = Resources.Load<SkinResources>("SkinResources");

    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    public List<SkinData> SkinDatas;
    [SerializeField, SpineSkin] private string skinNameDefault;

    public SkinData SkinDefault => SkinDatas.Find(item => item.SkinName == skinNameDefault);
    public List<SkinData> SkinsDailyReward => SkinDatas.FindAll(item => item.SkinType == SkinType.Daily);
    public List<SkinData> SkinsLocked => SkinDatas.FindAll(item => !item.IsUnlocked);
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
            Data.IdCheckSkinUnlocked = Id;
            return Data.IsSkinUnlocked;
        }

        set
        {
            Data.IdCheckSkinUnlocked = Id;
            Data.IsSkinUnlocked = value;
        }
    }
}

public enum SkinType
{
    Coin,
    Ads,
    Daily
}