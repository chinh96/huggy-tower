using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "UniverseResources", menuName = "ScriptableObjects/UniverseResources")]
public class UniverseResources : ScriptableObject
{
    public List<WorldResources> Worlds;

    public WorldResources WorldCurrent => Worlds.Find(item => item.WorldType == Data.WorldCurrent);

    public WorldResources WorldDefault => Worlds.Find(item => item.LevelUnlock == 0);

    public List<WorldResources> WorldsNotCurrent => Worlds.FindAll(item => item.WorldType != Data.WorldCurrent);

    public bool HasNotiUniverse
    {
        get
        {
            foreach (var world in Worlds)
            {
                if (world.IsUnlocked)
                {
                    foreach (var castle in world.Castles)
                    {
                        foreach (var data in castle.Castles)
                        {
                            if (!data.IsUnlocked && Data.CoinTotal >= data.Cost)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }

    public bool HasNotiWorld
    {
        get
        {
            foreach (var world in WorldsNotCurrent)
            {
                if (world.IsUnlocked)
                {
                    foreach (var castle in world.Castles)
                    {
                        foreach (var data in castle.Castles)
                        {
                            if (!data.IsUnlocked && Data.CoinTotal >= data.Cost)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }

    public bool HasNotiBuild
    {
        get
        {
            foreach (var castle in WorldCurrent.Castles)
            {
                foreach (var data in castle.Castles)
                {
                    if (!data.IsUnlocked && Data.CoinTotal >= data.Cost)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public bool IsNoti(WorldResources worldResources)
    {
        foreach (var castle in worldResources.Castles)
        {
            foreach (var data in castle.Castles)
            {
                if (!data.IsUnlocked && Data.CoinTotal >= data.Cost)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public string ConvertData()
    {
        DataModel[] models = new DataModel[Worlds.Count];

        for (int i = 0; i < Worlds.Count; i++)
        {
            models[i] = new DataModel() { data = Worlds[i].ConvertData() };
        }

        return JsonHelper.ToJson(models);
    }

    public void TransformData(string raw)
    {
        DataModel[] models = JsonHelper.FromJson<DataModel>(raw);
        
        int count = models.Length;
        if (count > Worlds.Count) count = Worlds.Count;
        
        for (int i = 0; i < count; i++)
        {
            Worlds[i].TransformData(models[i].data);
        }
    }
    
}
