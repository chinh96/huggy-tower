using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;

[CreateAssetMenu(fileName = "LibraryData", menuName = "ScriptableObjects/LibraryData")]
public class LibraryData : ScriptableObject, IHasSkeletonDataAsset
{
    public SkeletonDataAsset SkeDataAsset;
    public string Name;
    public int LevelUnlock;
    public Sprite Sprite;
    public string Description;
    public bool IsUnlocked => Data.CurrentLevel >= LevelUnlock;
    public SkeletonDataAsset SkeletonDataAsset => SkeDataAsset;
    public LibraryAnimation LibraryAnimation;
    public bool IsFlipX;
}

[Serializable]
public class LibraryAnimation
{
    [SpineAnimation] public string Idle;
}
