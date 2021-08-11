using com.adjust.sdk;
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

    public static void ClickDailyQuestButton()
    {
        LogEvent(Constants.CLICK_DAILY_QUEST_BUTTON, new Parameter[] { });
    }

    public static void ClaimDailyQuest()
    {
        LogEvent(Constants.CLAIM_DAILY_QUEST, new Parameter[] { });
    }

    public static void StartLevel1Funnel()
    {
        LogEvent(Constants.START_LEVEL_1_FUNNEL, new Parameter[] { });
    }

    public static void CompleteLevel1Funnel()
    {
        LogEvent(Constants.COMPLETE_LEVEL_1_FUNNEL, new Parameter[] { });
    }

    public static void StartLevel8Funnel()
    {
        LogEvent(Constants.START_LEVEL_8_FUNNEL, new Parameter[] { });
    }

    public static void ClickRankButton()
    {
        LogEvent(Constants.CLICK_RANK_BUTTON, new Parameter[] { });
    }

    public static void ClickDailyReward()
    {
        LogEvent(Constants.CLICK_DAILY_REWARD, new Parameter[] { });
    }

    public static void Level1StartFunnel()
    {
        LogEvent(Constants.LEVEL_1_START_FUNNEL, new Parameter[] { });
    }

    public static void Level1CompleteFunnel()
    {
        LogEvent(Constants.LEVEL_1_COMPLETE_FUNNEL, new Parameter[] { });
    }

    public static void BuildFirstKingdomItem()
    {
        LogEvent(Constants.BUILD_FIRST_KINGDOM_ITEM, new Parameter[] { });
    }

    public static void Level10StartFunnel()
    {
        LogEvent(Constants.LEVEL_10_START_FUNNEL, new Parameter[] { });
    }

    public static void Level10CompleteFunnel()
    {
        LogEvent(Constants.LEVEL_10_COMPLETE_FUNNEL, new Parameter[] { });
    }

    public static void ClaimFirstDailyQuest()
    {
        LogEvent(Constants.CLAIM_FIRST_DAILY_QUEST, new Parameter[] { });
    }

    public static void Level20StartFunnel()
    {
        LogEvent(Constants.LEVEL_20_START_FUNNEL, new Parameter[] { });
    }

    private static void LogEvent(string name, Parameter[] param)
    {
        FirebaseAnalytics.LogEvent(name, param);
    }

    
    #region adjust

    public static void AdjustLogEventPurchaseItem(string token, double revenue, string currency, string transactionId)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        adjustEvent.setRevenue(revenue, currency);
        adjustEvent.setTransactionId(transactionId);
        
        Adjust.trackEvent(adjustEvent);
    }

    public static void AdjustLogEventFirstOpen()
    {
        AdjustEvent adjustEvent = new AdjustEvent("2ng43u");
        Adjust.trackEvent(adjustEvent);
    }

    public static void AdjustLogEventPlayLevel1()
    {
        AdjustEvent adjustEvent = new AdjustEvent("stigq2");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel10()
    {
        AdjustEvent adjustEvent = new AdjustEvent("bop09g");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel2()
    {
        AdjustEvent adjustEvent = new AdjustEvent("u1xwya");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel3()
    {
        AdjustEvent adjustEvent = new AdjustEvent("947epe");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel4()
    {
        AdjustEvent adjustEvent = new AdjustEvent("i0iqy6");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel5()
    {
        AdjustEvent adjustEvent = new AdjustEvent("75zm9b");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel6()
    {
        AdjustEvent adjustEvent = new AdjustEvent("7ki2zj");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel7()
    {
        AdjustEvent adjustEvent = new AdjustEvent("pfkw26");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel8()
    {
        AdjustEvent adjustEvent = new AdjustEvent("h692zz");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventPlayLevel9()
    {
        AdjustEvent adjustEvent = new AdjustEvent("ft60rt");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventClaimDailyReward()
    {
        AdjustEvent adjustEvent = new AdjustEvent("8hgi5a");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventClaimDailyRewardByAds()
    {
        AdjustEvent adjustEvent = new AdjustEvent("o9qvze");
        Adjust.trackEvent(adjustEvent);
    }

    public static void AdjustLogEventClaimX5CoinWinLevel()
    {
        AdjustEvent adjustEvent = new AdjustEvent("tdpoes");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventSkipLoseLevel()
    {
        AdjustEvent adjustEvent = new AdjustEvent("qwz9ld");
        Adjust.trackEvent(adjustEvent);
    }

    public static void AdjustLogEventClaimGiftProcessWinLevel()
    {
        AdjustEvent adjustEvent = new AdjustEvent("eqxbmi");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventShareFb()
    {
        AdjustEvent adjustEvent = new AdjustEvent("yuoia3");
        Adjust.trackEvent(adjustEvent);
    }
    
    public static void AdjustLogEventBuildCastle()
    {
        AdjustEvent adjustEvent = new AdjustEvent("skrw1k");
        Adjust.trackEvent(adjustEvent);
    }


    #endregion
}