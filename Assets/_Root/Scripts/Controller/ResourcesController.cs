using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourcesController : Singleton<ResourcesController>
{
    [SerializeField] private SkinResources HeroResources;
    [SerializeField] private SkinResources PrincessResources;
    [SerializeField] private SoundResources SoundResources;
    [SerializeField] private UniverseResources UniverseResources;
    [SerializeField] private DailyRewardResources DailyRewardResources;

    public static SkinResources Hero;
    public static SkinResources Princess;
    public static SoundResources Sound;
    public static UniverseResources Universe;
    public static DailyRewardResources DailyReward;

    private void OnEnable()
    {
        Hero = HeroResources;
        Princess = PrincessResources;
        Sound = SoundResources;
        Universe = UniverseResources;
        DailyReward = DailyRewardResources;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Hero.SkinDefault.IsUnlocked = true;
    }
}
