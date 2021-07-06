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

    public void CheckAchievementDailyQuest(int value)
    {
        CheckAchievement(value);
        CheckDailyQuest(value);
    }

    public void CheckAchievement(int value)
    {
        foreach (var world in Worlds)
        {
            if (value >= world.LevelUnlock)
            {
                switch (world.WorldType)
                {
                    case WorldType.Earth:
                        ResourcesController.Achievement.IncreaseByType(AchievementType.CompleteEarth);
                        break;
                    case WorldType.Desert:
                        ResourcesController.Achievement.IncreaseByType(AchievementType.CompleteDesert);
                        break;
                    case WorldType.Iceland:
                        ResourcesController.Achievement.IncreaseByType(AchievementType.CompleteIceland);
                        break;
                    case WorldType.Inferno:
                        ResourcesController.Achievement.IncreaseByType(AchievementType.CompleteInferno);
                        break;
                    case WorldType.Jade:
                        ResourcesController.Achievement.IncreaseByType(AchievementType.CompleteJade);
                        break;
                    case WorldType.Olympus:
                        ResourcesController.Achievement.IncreaseByType(AchievementType.CompleteOlympus);
                        break;
                }
            }
        }
    }

    public void CheckDailyQuest(int value)
    {
        foreach (var world in Worlds)
        {
            if (value >= world.LevelUnlock)
            {
                switch (world.WorldType)
                {
                    case WorldType.Earth:
                        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.CompleteEarth);
                        break;
                    case WorldType.Desert:
                        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.CompleteDesert);
                        break;
                    case WorldType.Iceland:
                        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.CompleteIceland);
                        break;
                    case WorldType.Inferno:
                        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.CompleteInferno);
                        break;
                    case WorldType.Jade:
                        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.CompleteJade);
                        break;
                    case WorldType.Olympus:
                        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.CompleteOlympus);
                        break;
                }
            }
        }
    }
}
