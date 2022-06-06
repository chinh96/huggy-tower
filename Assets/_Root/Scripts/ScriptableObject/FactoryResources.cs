using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "FactoryResources", menuName = "ScriptableObjects/FactoryResources")]
public class FactoryResources : ScriptableObject
{
    public List<RoomResources> Rooms;

    public RoomResources RoomCurrent => Rooms.Find(item => item.roomType == Data.RoomCurrent);

    public RoomResources RoomDefault => Rooms.Find(item => item.isDefaultRoom);

    public List<RoomResources> RoomsNotCurrent => Rooms.FindAll(item => item.roomType != Data.RoomCurrent);
    
    public string ConvertData()
    {
        DataModel[] models = new DataModel[Rooms.Count];

        for (int i = 0; i < Rooms.Count; i++)
        {
            models[i] = new DataModel() { data = Rooms[i].ConvertData() };
        }

        return JsonHelper.ToJson(models);
    }

    public void TransformData(string raw)
    {
        DataModel[] models = JsonHelper.FromJson<DataModel>(raw);
        
        int count = models.Length;
        if (count > Rooms.Count) count = Rooms.Count;
        
        for (int i = 0; i < count; i++)
        {
            Rooms[i].TransformData(models[i].data);
        }
    }
}
