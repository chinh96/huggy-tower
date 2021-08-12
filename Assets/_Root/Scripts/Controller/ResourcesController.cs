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

    public static List<SkinData> SkinRescuePartys = new List<SkinData>();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        Init();

        if (Data.DateTimeStart == "")
        {
            Data.DateTimeStart = DateTime.Now.ToString();
        }

        if (Data.DateTimeStartRescueParty == "")
        {
            Data.DateTimeStartRescueParty = DateTime.Now.ToString();
        }

        GetSkinRescueParty();
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
        Princess.Reset();
        DailyQuest.Reset();
        Library.Reset();
    }

    public static void GetSkinRescueParty()
    {
        SkinRescuePartys.Add(Princess.SkinDatas.Find(data => data.RescuePartyType == RescuePartyType.Princess));
        SkinRescuePartys.Add(Hero.SkinDatas.Find(data => data.RescuePartyType == RescuePartyType.Hero));
        SkinRescuePartys.Add(Hero.SkinDatas.Find(data => data.RescuePartyType == RescuePartyType.Top100));
    }

    public static void ReceiveSkinRescueParty(Action action)
    {
        if (Data.TimeToRescueParty.TotalMilliseconds <= 0)
        {
            SkinData data = SkinRescuePartys.Find(data => data.RescuePartyType == RescuePartyType.Top100);
            if (!data.IsUnlocked)
            {
                LeaderboardRescuePartyController.Instance.IsTop100(() =>
                {
                    action?.Invoke();
                });
            }
        }
    }
}
