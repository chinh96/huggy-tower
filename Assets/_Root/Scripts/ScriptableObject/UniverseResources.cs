using System.Collections;
using System.Collections.Generic;
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
}
