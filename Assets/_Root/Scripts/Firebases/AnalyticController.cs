using Firebase.Analytics;

public static class AnalyticController
{
    public static void StartLevel()
    {
        LogEvent(Constants.LEVEL_STARTED, new Parameter[] {
            new Parameter(Constants.LEVEL_INDEX_NAME, $"Level_index-{Data.CurrentLevel}_name-{GameController.Instance.Root.LevelMap.gameObject.name}")
        });
    }

    public static void CompleteLevel()
    {
        LogEvent(Constants.LEVEL_COMPLETED, new Parameter[] {
            new Parameter(Constants.LEVEL_INDEX_NAME, $"Level_index-{Data.CurrentLevel}_name-{GameController.Instance.Root.LevelMap.gameObject.name}")
        });
    }

    public static void FailLevel()
    {
        LogEvent(Constants.LEVEL_FAILED, new Parameter[] {
            new Parameter(Constants.LEVEL_INDEX_NAME, $"Level_index-{Data.CurrentLevel}_name-{GameController.Instance.Root.LevelMap.gameObject.name}")
        });
    }

    public static void ImpressAdReward()
    {
        LogEvent(Constants.AD_REWARD_IMPRESSION, new Parameter[] { });
    }

    public static void RequestAdReward()
    {
        LogEvent(Constants.AD_REWARD_REQUEST, new Parameter[] { });
    }

    public static void ImpressAdInterstitial()
    {
        LogEvent(Constants.AD_INTERSTITIAL_IMPRESSION, new Parameter[] { });
    }

    public static void RequestAdInterstitial()
    {
        LogEvent(Constants.AD_INTERSTITIAL_REQUEST, new Parameter[] { });
    }

    public static void UnlockSkinVideo()
    {
        LogEvent(Constants.UNLOCK_SKIN_VIDEO, new Parameter[] { });
    }

    public static void UnlockSkinCoins()
    {
        LogEvent(Constants.UNLOCK_SKIN_COINS, new Parameter[] { });
    }

    public static void UnlockSkinFacebook()
    {
        LogEvent(Constants.UNLOCK_SKIN_FACEBOOK, new Parameter[] { });
    }

    public static void BuildCastle()
    {
        LogEvent(Constants.BUILD_CASTLE, new Parameter[] { });
    }

    public static void SkipLevel()
    {
        LogEvent(Constants.SKIP_LEVEL, new Parameter[] { });
    }

    public static void ClickDailyquestButton()
    {
        LogEvent(Constants.CLICK_DAILY_QUEST_BUTTON, new Parameter[] { });
    }

    public static void ClaimDailyQuest()
    {
        LogEvent(Constants.CLAIM_DAILY_QUEST, new Parameter[] { });
    }

    private static void LogEvent(string name, Parameter[] param)
    {
        FirebaseAnalytics.LogEvent(name, param);
    }
}