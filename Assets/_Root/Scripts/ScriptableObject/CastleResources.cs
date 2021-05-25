using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CastleResources", menuName = "ScriptableObjects/CastleResources")]
public class CastleResources : ScriptableObject
{
    public List<CastleData> Castles;

    public CastleData CastleCurrent
    {
        get
        {
            CastleData result = null;
            foreach (CastleData item in Castles)
            {
                if (item.IsUnlocked)
                {
                    result = item;
                }
            }

            return result;
        }
    }
}

[Serializable]
public class CastleData
{
    [GUID] public string Id;
    public Sprite Sprite;
    public int Cost;
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