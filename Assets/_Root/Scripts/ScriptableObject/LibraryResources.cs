using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spine.Unity;

[CreateAssetMenu(fileName = "LibraryResources", menuName = "ScriptableObjects/LibraryResources")]
public class LibraryResources : ScriptableObject
{
    public List<LibraryData> LibraryDatas;
}

[Serializable]
public class LibraryData
{
    public int LevelUnlock;
    public Sprite Sprite;
    public SkeletonDataAsset skeletonDataAsset;
    public string Name;
    public string Description;
    public bool IsUnlocked => Data.CurrentLevel >= LevelUnlock;
}
