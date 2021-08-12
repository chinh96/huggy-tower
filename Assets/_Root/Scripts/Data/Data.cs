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

    public static string PlayerId
    {
        get => GetString(Constants.PLAYER_ID, "");
        set
        {
            SetString(Constants.PLAYER_ID, value);
            EventController.LoginLeaderBoard();
        }
    }

    public static int CurrentLevel
    {
        get => GetInt(Constants.CURRENT_LEVEL, 0);
        set
        {
            ResourcesController.Achievement.IncreaseByType(AchievementType.PlayToLevel);

            ResourcesController.Achievement.UpdateNumberCurrent();
            ResourcesController.DailyQuest.UpdateNumberCurrent();

            SetInt(Constants.CURRENT_LEVEL, value);
        }
    }

    public static int MaxLevel { get => GetInt(Constants.MAX_LEVEL, 0); set => SetInt(Constants.MAX_LEVEL, value); }

    public static string CurrentSkinHero
    {
        get => GetString(Constants.CURRENT_SKIN_HERO, ResourcesController.Hero.SkinDefault.SkinName);
        set
        {
            SetString(Constants.CURRENT_SKIN_HERO, value);
            EventController.CurrentSkinHeroChanged?.Invoke();
        }
    }

    public static string CurrentSkinPrincess
    {
        get => GetString(Constants.CURRENT_SKIN_PRINCESS, ResourcesController.Princess.SkinDefault.SkinName);
        set
        {
            SetString(Constants.CURRENT_SKIN_PRINCESS, value);
            EventController.CurrentSkinPrincessChanged?.Invoke();
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

    public static WorldType WorldCurrent { get => (WorldType)GetInt(Constants.WORLD_CURRENT, (int)(ResourcesController.Universe.WorldDefault.WorldType)); set => SetInt(Constants.WORLD_CURRENT, (int)value); }

    public static SkinData SkinGift;

    public static bool DoneOnboarding { get => GetBool(Constants.DONE_ONBOARDING, false); set => SetBool(Constants.DONE_ONBOARDING, value); }

    public static int JoinFbProgress { get => GetInt(Constants.JOIN_FB_PROGRESS, 0); set => SetInt(Constants.JOIN_FB_PROGRESS, value); }

    public static string AchievementId;
    public static int AchievementNumberTarget { get => GetInt(Constants.ACHIEVEMENT_NUMBER_TARGET + AchievementId, 10); set => SetInt(Constants.ACHIEVEMENT_NUMBER_TARGET + AchievementId, value); }
    public static int AchievementNumberCurrent { get => GetInt(Constants.ACHIEVEMENT_NUMBER_CURRENT + AchievementId, 0); set => SetInt(Constants.ACHIEVEMENT_NUMBER_CURRENT + AchievementId, value); }

    public static int TotalDays => (int)(DateTime.Now - DateTime.Parse(DateTimeStart)).TotalDays;

    public static string DailyQuestId;
    public static int DailyQuestNumberCurrent { get => GetInt(Constants.DAILY_QUEST_NUMBER_CURRENT + DailyQuestId, 0); set => SetInt(Constants.DAILY_QUEST_NUMBER_CURRENT + DailyQuestId, value); }

    public static LibraryData LibraryItemInfo;

    public static bool DontShowUpdateAgain { get => GetBool(Constants.DONT_SHOW_UPDATE_AGAIN, false); set => SetBool(Constants.DONT_SHOW_UPDATE_AGAIN, value); }

    public static bool IsBuildFirstKingdomItem { get => GetBool(Constants.BUILD_FIRST_KINGDOM_ITEM, false); set => SetBool(Constants.BUILD_FIRST_KINGDOM_ITEM, value); }

    public static bool IsClaimFirstDailyQuest { get => GetBool(Constants.CLAIM_FIRST_DAILY_QUEST, false); set => SetBool(Constants.CLAIM_FIRST_DAILY_QUEST, value); }

    public static bool FirstOpen { get => GetBool(Constants.FIRST_OPEN, false); set => SetBool(Constants.FIRST_OPEN, value); }

    public static bool FlagPlayLevel1 { get => GetBool(Constants.PLAY_LEVEL_1, false); set => SetBool(Constants.PLAY_LEVEL_1, value); }

    public static bool FlagPlayLevel2 { get => GetBool(Constants.PLAY_LEVEL_2, false); set => SetBool(Constants.PLAY_LEVEL_2, value); }

    public static bool FlagPlayLevel3 { get => GetBool(Constants.PLAY_LEVEL_3, false); set => SetBool(Constants.PLAY_LEVEL_3, value); }

    public static bool FlagPlayLevel4 { get => GetBool(Constants.PLAY_LEVEL_4, false); set => SetBool(Constants.PLAY_LEVEL_4, value); }

    public static bool FlagPlayLevel5 { get => GetBool(Constants.PLAY_LEVEL_5, false); set => SetBool(Constants.PLAY_LEVEL_5, value); }

    public static bool FlagPlayLevel6 { get => GetBool(Constants.PLAY_LEVEL_6, false); set => SetBool(Constants.PLAY_LEVEL_6, value); }

    public static bool FlagPlayLevel7 { get => GetBool(Constants.PLAY_LEVEL_7, false); set => SetBool(Constants.PLAY_LEVEL_7, value); }

    public static bool FlagPlayLevel8 { get => GetBool(Constants.PLAY_LEVEL_8, false); set => SetBool(Constants.PLAY_LEVEL_8, value); }

    public static bool FlagPlayLevel9 { get => GetBool(Constants.PLAY_LEVEL_9, false); set => SetBool(Constants.PLAY_LEVEL_9, value); }

    public static bool FlagPlayLevel10 { get => GetBool(Constants.PLAY_LEVEL_10, false); set => SetBool(Constants.PLAY_LEVEL_10, value); }
    public static bool FlagPlayLevel20 { get => GetBool(Constants.PLAY_LEVEL_20, false); set => SetBool(Constants.PLAY_LEVEL_20, value); }
    public static bool FlagFirstTimeVisitCastle { get => GetBool(Constants.FIRST_TIME_VISIT_CASTLE, false); set => SetBool(Constants.FIRST_TIME_VISIT_CASTLE, value); }
    
    public static int TotalGoldMedal
    {
        get => GetInt(Constants.TOTAL_GOLD_MEDAL, 0);
        set
        {
            SetInt(Constants.TOTAL_GOLD_MEDAL, value);
            EventController.MedalTotalChanged?.Invoke();
        }
    }

    public static TimeSpan TimeToRescueParty => new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) - DateTime.Now;

    public static string DateTimeStartRescueParty { get => GetString(Constants.DATE_TIME_START_RESCUE_PARTY, ""); set => SetString(Constants.DATE_TIME_START_RESCUE_PARTY, value); }

    public static bool ClickedTop100Button { get => GetBool(Constants.CLICKED_TOP_100_BUTTON, false); set => SetBool(Constants.CLICKED_TOP_100_BUTTON, value); }
}