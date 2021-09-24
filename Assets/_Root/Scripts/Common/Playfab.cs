using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public static class Playfab
{
    public static string TitleId = "4D244";

    public static int MaxResultsCount = 100;

    public static void Login(Action<LoginResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) PlayFabSettings.staticSettings.TitleId = TitleId;

        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest { CustomId = Data.CustomId, TitleId = TitleId, CreateAccount = true },
            callbackResult,
            callbackError
        );
    }

    public static void UpdateDisplayName(string displayName, Action<UpdateUserTitleDisplayNameResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest { DisplayName = displayName },
            callbackResult,
            callbackError
        );
    }

    public static void UpdateScore(int score, string nameTable, Action<UpdatePlayerStatisticsResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(
            new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = nameTable, Value = score } } },
            callbackResult,
            callbackError
        );
    }

    public static void GetPlayerProfile(Action<GetPlayerProfileResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), callbackResult, callbackError);
    }

    public static void GetLeaderboard(string nameTable, int startPosition, Action<GetLeaderboardResult> callbackResult = null, Action<PlayFabError> callbackError = null)
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

    public static void LoginWithFacebook(string tokenId, Action<LoginResult> onLoginSuccessFacebook)
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) PlayFabSettings.staticSettings.TitleId = TitleId;

        var request = new LoginWithFacebookRequest() { AccessToken = tokenId, CreateAccount = true, };
        PlayFabClientAPI.LoginWithFacebook(request, onLoginSuccessFacebook, error => { });
    }
}