using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldResources", menuName = "ScriptableObjects/WorldResources")]
public class WorldResources : ScriptableObject
{
    public WorldType WorldType;
    public int LevelUnlock;
    public List<CastleResources> Castles;
    public bool IsUnlocked => Data.CurrentLevel >= LevelUnlock;
    public Sprite background;
    public bool IsComplete => Castles.TrueForAll(item => item.IsComplete);
    
    public string ConvertData()
    {
        StringBuilder result = new StringBuilder("");
        for (int i = 0; i < Castles.Count; i++)
        {
            result.Append($"{Castles[i].ConvertData()}|");
        }

        result.Remove(result.Length - 1, 1);
        return result.ToString();
    }

    public void TransformData(string raw)
    {
        string[] result = raw.Split('|');
        
        int count = result.Length;
        if (count > Castles.Count) count = Castles.Count;

        for (int i = 0; i < count; i++)
        {
            Castles[i].TransformData(result[i]);
        }
    }
}