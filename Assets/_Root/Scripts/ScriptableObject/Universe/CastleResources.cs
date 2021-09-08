using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

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

    public bool IsComplete => Castles.TrueForAll(item => item.IsUnlocked);
    
    public string ConvertData()
    {
        StringBuilder result = new StringBuilder("");
        for (int i = 0; i < Castles.Count; i++)
        {
            result.Append($"{Castles[i].IsUnlocked}@");
        }

        result.Remove(result.Length - 1, 1);
        return result.ToString();
    }

    public void TransformData(string raw)
    {
        var result = raw.Split('@');
        int count = result.Length;
        if (count > Castles.Count) count = Castles.Count;

        for (int i = 0; i < count; i++)
        {
            Castles[i].IsUnlocked = bool.Parse(result[i].ToLower());
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