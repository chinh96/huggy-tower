using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;

[CreateAssetMenu(fileName = "LibraryData", menuName = "ScriptableObjects/LibraryData")]
public class LibraryData : ScriptableObject, IHasSkeletonDataAsset
{
    public Sprite Image;
    public SkeletonDataAsset SkeDataAsset;
    public string Name;
    public int LevelUnlock;
    public Sprite Sprite;
    public string Description;
    public bool IsUnlocked => Data.CurrentLevel >= LevelUnlock;
    public SkeletonDataAsset SkeletonDataAsset => SkeDataAsset;
    public LibrarySkeletonData LibraryAnimation;
    public bool IsFlipX;
    public Vector3 Scale = Vector3.one * 1.5f;
    public Vector3 Offset = new Vector3(-175, 200, 0);
    public Vector3 Rotation = Vector3.zero;
    public bool IsFlipX2;
    public Vector3 Scale2 = Vector3.one * 1.5f;
    public Vector3 Offset2 = new Vector3(0, 0, 0);
}

[Serializable]
public class LibrarySkeletonData
{
    [SpineAnimation] public string Idle;
    [SpineSkin] public string SkinName;
}
