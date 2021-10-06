using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SoundResources", menuName = "ScriptableObjects/SoundResources")]
public class SoundResources : ScriptableObject
{
    public List<SoundData> SoundDatas;
}

[Serializable]
public class SoundData
{
    [Enum]
    public SoundType SoundType;
    public AudioClip Clip;
}
