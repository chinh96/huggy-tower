using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

public class HeroSkinData : ScriptableObject, IHasSkeletonDataAsset
{
    private static HeroSkinData instance;
    public static HeroSkinData Instance => instance ? instance : instance = Resources.Load<HeroSkinData>("HeroSkinData");

    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

    public string SkinHeroByIndex(int index) { return $"Hero{index + 1}"; }

    public List<HeroSkin> heroSkins;
}



[Serializable]
public class HeroSkin
{
    [GUID] public string id;

    public HeroSkinType heroSkinType;

    [SpineSkin] public string skinName;
}

public enum HeroSkinType
{
    Coin,
    Ads,
    Daily
}