using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

[CreateAssetMenu(fileName = "FunitureResources", menuName = "ScriptableObjects/FunirureResources")]
public class FurnitureResources : ScriptableObject
{
    public List<FurnitureData> FurnitureLevels;

    public FurnitureData FurnitureCurrent
    {
        get
        {
            FurnitureData result = null;
            foreach (FurnitureData funiture in FurnitureLevels)
            {
                if (funiture.IsUnlocked)
                {
                    result = funiture;
                }
            }
            return result;
        }
    }

    public void Upgrade()
    {
        foreach (FurnitureData item in FurnitureLevels)
        {
            if (!item.IsUnlocked)
            {
                item.IsUnlocked = true;
                break;
            }
        }
    }

    public bool IsComplete => FurnitureLevels.TrueForAll(item => item.IsUnlocked);
    
    public string ConvertData()
    {
        StringBuilder result = new StringBuilder("");
        for (int i = 0; i < FurnitureLevels.Count; i++)
        {
            result.Append($"{FurnitureLevels[i].IsUnlocked}@");
        }

        result.Remove(result.Length - 1, 1);
        return result.ToString();
    }

    public void TransformData(string raw)
    {
        var result = raw.Split('@');
        int count = result.Length;
        if (count > FurnitureLevels.Count) count = FurnitureLevels.Count;

        for (int i = 0; i < count; i++)
        {
            FurnitureLevels[i].IsUnlocked = bool.Parse(result[i].ToLower());
        }
    }
}

[Serializable]
public class FurnitureData
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