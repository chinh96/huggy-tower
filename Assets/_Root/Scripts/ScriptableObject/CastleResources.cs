using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CastleResources", menuName = "ScriptableObjects/CastleResources")]
public class CastleResources : ScriptableObject
{
    public List<CastleData> Castles;
}

[Serializable]
public class CastleData
{
    [GUID] public string Id;
    public Sprite sprite;

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