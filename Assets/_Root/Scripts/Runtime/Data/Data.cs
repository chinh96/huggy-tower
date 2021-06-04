using System;
using UnityEngine;

public static class Data
{
    private static bool GetBool(string key, bool defaultValue = false) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) > 0;
    private static void SetBool(string id, bool value) => PlayerPrefs.SetInt(id, value ? 1 : 0);

    private static int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
    private static void SetInt(string id, int value) => PlayerPrefs.SetInt(id, value);

    private static string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
    private static void SetString(string id, string value) => PlayerPrefs.SetString(id, value);

    public static int CurrentLevel { get => GetInt(Constants.CURRENT_LEVEL, 0); set => SetInt(Constants.CURRENT_LEVEL, value); }

    public static int MaxLevel { get => GetInt(Constants.MAX_LEVEL, 0); set => SetInt(Constants.MAX_LEVEL, value); }

    public static string CurrentSkinHero
    {
        get => GetString(Constants.CURRENT_SKIN_HERO, ResourcesController.Instance.Hero.SkinDefault.SkinName);
        set
        {
            SetString(Constants.CURRENT_SKIN_HERO, value);
            EventController.CurrentSkinHeroChanged();
        }
    }

    public static string CurrentSkinPrincess
    {
        get => GetString(Constants.CURRENT_SKIN_PRINCESS, "");
        set
        {
            SetString(Constants.CURRENT_SKIN_PRINCESS, value);
            EventController.CurrentSkinPrincessChanged();
        }
    }

    public static bool SoundState { get => GetBool(Constants.SOUND_STATE, true); set => SetBool(Constants.SOUND_STATE, value); }

    public static bool MusicState { get => GetBool(Constants.MUSIC_STATE, true); set => SetBool(Constants.MUSIC_STATE, value); }

    public static bool VibrateState { get => GetBool(Constants.VIBRATE_STATE, false); set => SetBool(Constants.VIBRATE_STATE, value); }

    public static bool IsRemovedAds { get => GetBool(Constants.IS_REMOVE_ADS, false); set => SetBool(Constants.IS_REMOVE_ADS, value); }

    public static bool IsUnlockAllSkins { get => GetBool(Constants.IS_UNLOCK_ALL_SKINS, false); set => SetBool(Constants.IS_UNLOCK_ALL_SKINS, value); }

    public static bool IsVip { get => GetBool(Constants.IS_VIP, false); set => SetBool(Constants.IS_VIP, value); }

    public static int GetCacheLevelIndex(int index) { return GetInt($"{Constants.CACHE_LEVEL_INDEX}_{index}", 0); }
    public static void SetCacheLevelIndex(int index, int level) { SetInt($"{Constants.CACHE_LEVEL_INDEX}_{index}", level); }

    public static int CountPlayLevel { get => GetInt(Constants.COUNT_PLAY_LEVEL, 0); set => SetInt(Constants.COUNT_PLAY_LEVEL, value); }

    public static int CoinTotal
    {
        get => GetInt(Constants.COIN_TOTAL, 0);
        set
        {
            SetInt(Constants.COIN_TOTAL, value);
            EventController.CoinTotalChanged();
        }
    }

    public static string DateTimeStart { get => GetString(Constants.DATE_TIME_START, ""); set => SetString(Constants.DATE_TIME_START, value); }

    public static int DailyRewardCurrent { get => GetInt(Constants.DAILY_REWARD_CURRENT, 0); set => SetInt(Constants.DAILY_REWARD_CURRENT, value); }

    public static string IdCheckUnlocked = "";
    public static bool IsUnlocked { get => GetBool(IdCheckUnlocked, false); set => SetBool(IdCheckUnlocked, value); }

    public static int PercentProgressGift { get => GetInt(Constants.PERCENT_PROGRESS_GIFT, 0); set => SetInt(Constants.PERCENT_PROGRESS_GIFT, value); }

    public static WorldType WorldCurrent { get => (WorldType)GetInt(Constants.WORLD_CURRENT, (int)(UniverseResources.Instance.WorldDefault.WorldType)); set => SetInt(Constants.WORLD_CURRENT, (int)value); }

    public static SkinData SkinGift;

    public static bool DoneOnboarding { get => GetBool(Constants.DONE_ONBOARDING, false); set => SetBool(Constants.DONE_ONBOARDING, value); }
}