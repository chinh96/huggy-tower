using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomResources", menuName = "ScriptableObjects/RoomResources")]
public class RoomResources : ScriptableObject
{
    public  RoomType roomType;
    public List<FurnitureResources> Funitures;
    public Sprite upgradedFrame;
    public Sprite unUpgradedFrame;
    public bool isDefaultRoom = false;
    public bool IsComplete => Funitures.TrueForAll(item => item.IsComplete);
    public string ConvertData()
    {
        StringBuilder result = new StringBuilder("");
        for (int i = 0; i < Funitures.Count; i++)
        {
            result.Append($"{Funitures[i].ConvertData()}|");
        }

        result.Remove(result.Length - 1, 1);
        return result.ToString();
    }

    public void TransformData(string raw)
    {
        string[] result = raw.Split('|');
        
        int count = result.Length;
        if (count > Funitures.Count) count = Funitures.Count;

        for (int i = 0; i < count; i++)
        {
            Funitures[i].TransformData(result[i]);
        }
    }
}