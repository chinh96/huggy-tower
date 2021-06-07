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
            foreach (CastleData castle in Castles)
            {
                if (castle.IsUnlocked)
                {
                    result = castle;
                }
            }
            return result;
        }
    }

    public void BuildOrUpgrade()
    {
        foreach (CastleData item in Castles)
        {
            if (!item.IsUnlocked)
            {
                item.IsUnlocked = true;
                break;
            }
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