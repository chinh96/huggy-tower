using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class HeroSkinData : ScriptableObject
{
    public SkeletonDataAsset heroAsset;
    private static HeroSkinData instance;
    public List<WeaponData> weapons;
    public static HeroSkinData Instance => instance ? instance : instance = Resources.Load<HeroSkinData>("HeroSkinData");

    public static string SkinHeroByIndex(int index) { return $"Hero{index + 1}"; }
}


[Serializable]
public class WeaponData
{
    public string weaponSkin;
}