using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public static class Playfab
{
    private static string nameTable = "HERO_TOWER_WAR";
    private static string titleId = "4D244";

    public static int MaxResultsCount = 100;

    public static void Login(Action<LoginResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) PlayFabSettings.staticSettings.TitleId = titleId;

        try
        {
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceUniqueIdentifier, TitleId = titleId, CreateAccount = true },
                callbackResult,
                callbackError
            );
        }
        catch (Exception e)
        {

        }
    }

    public static void UpdateDisplayName(string displayName, Action<UpdateUserTitleDisplayNameResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        try
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest { DisplayName = displayName },
                callbackResult,
                callbackError
            );
        }
        catch (Exception e)
        {

        }
    }

    public static void UpdateScore(int score, Action<UpdatePlayerStatisticsResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        try
        {
            PlayFabClientAPI.UpdatePlayerStatistics(
                new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = nameTable, Value = score } } },
                callbackResult,
                callbackError
            );
        }
        catch (Exception e)
        {

        }
    }

    public static void GetPlayerProfile(Action<GetPlayerProfileResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        try
        {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), callbackResult, callbackError);
        }
        catch (Exception e)
        {

        }
    }

    public static void GetLeaderboard(int startPosition, Action<GetLeaderboardResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        try
        {
            PlayFabClientAPI.GetLeaderboard(
                new GetLeaderboardRequest
                {
                    StatisticName = nameTable,
                    StartPosition = startPosition,
                    MaxResultsCount = MaxResultsCount,
                    ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true, ShowLocations = true }
                },
                callbackResult,
                callbackError
            );
        }
        catch (Exception e)
        {

        }
    }
}