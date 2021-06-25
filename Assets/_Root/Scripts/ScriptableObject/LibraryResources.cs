using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spine.Unity;

[CreateAssetMenu(fileName = "LibraryResources", menuName = "ScriptableObjects/LibraryResources")]
public class LibraryResources : ScriptableObject
{
    public List<LibraryData> LibraryDatas;

    public void Reset()
    {
        LibraryDatas.Sort((a, b) => a.LevelUnlock.CompareTo(b.LevelUnlock));
    }
}
