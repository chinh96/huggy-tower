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
    [SerializeField] private LibraryResources library;
    [SerializeField] private CountryResources country;

    public static SkinResources Hero;
    public static SkinResources Princess;
    public static SoundResources Sound;
    public static UniverseResources Universe;
    public static DailyRewardResources DailyReward;
    public static QuestResources Quest;
    public static ConfigResources Config;
    public static AchievementResources Achievement;
    public static DailyQuestResources DailyQuest;
    public static LibraryResources Library;
    public static CountryResources Country;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        Init();

        if (Data.DateTimeStart == "")
        {
            Data.DateTimeStart = DateTime.Now.ToString();
        }
    }

    private void Start()
    {
        Reset();
    }

    private void Init()
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
        Library = library;
        Country = country;
    }

    private void Reset()
    {
        Hero.Reset();
        DailyQuest.Reset();
        Library.Reset();
    }
}
