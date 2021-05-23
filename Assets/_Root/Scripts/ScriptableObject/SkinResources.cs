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

    [SerializeField] private List<SkinData> SkinDatas;

    public SkinData GetSkinDataByName(string skinName)
    {
        return SkinDatas.Find(item => item.SkinName == skinName);
    }
}



[Serializable]
public class SkinData
{
    [GUID] public string Id;

    public SkinType SkinType;

    [SpineSkin] public string SkinName;

    public int Coin;

    public bool IsUnlocked
    {
        get
        {
            Data.IdCheckSkinUnlocked = Id;
            return Data.IsSkinUnlocked;
        }
    }
}

public enum SkinType
{
    Coin,
    Ads,
    Daily
}