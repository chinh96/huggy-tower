using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourcesController : Singleton<ResourcesController>
{
    [SerializeField] private SkinResources heroResources;
    [SerializeField] private SkinResources princessResources;
    [SerializeField] private SoundResources soundResources;
    [SerializeField] private UniverseResources universeResources;
    [SerializeField] private DailyRewardResources dailyRewardResources;
    [SerializeField] private QuestResources questResources;
    [SerializeField] private ConfigResources config;
    [SerializeField] private AchievementResources achievement;
    [SerializeField] private DailyQuestResources dailyQuest;

    public static SkinResources Hero;
    public static SkinResources Princess;
    public static SoundResources Sound;
    public static UniverseResources Universe;
    public static DailyRewardResources DailyReward;
    public static QuestResources Quest;
    public static ConfigResources Config;
    public static AchievementResources Achievement;
    public static DailyQuestResources DailyQuest;

    private void OnEnable()
    {
        Hero = heroResources;
        Princess = princessResources;
        Sound = soundResources;
        Universe = universeResources;
        DailyReward = dailyRewardResources;
        Quest = questResources;
        Config = config;
        Achievement = achievement;
        DailyQuest = dailyQuest;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Hero.SkinDefault.IsUnlocked = true;
    }
}
