using System;
using System.Globalization;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LeaderboardController : Singleton<LeaderboardController>
{
    [NonSerialized] public LeaderboardUserInfo UserInfoCurrent = new LeaderboardUserInfo();
    [NonSerialized] public bool IsWorldTab = true;

    private int startPosition = -1;
    private List<LeaderboardUserInfo> userInfosAllTab = new List<LeaderboardUserInfo>();
    private List<LeaderboardUserInfo> userInfosWorldTab = new List<LeaderboardUserInfo>();
    private List<LeaderboardUserInfo> userInfosCountryTab = new List<LeaderboardUserInfo>();
    public List<LeaderboardUserInfo> UserInfos => IsWorldTab ? userInfosWorldTab : userInfosCountryTab;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        FetchInfo();
    }

    private void FetchInfo()
    {
        if (Data.PlayerId == "")
        {
            string code = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            CountryData countryData = ResourcesController.Country.GetDataByCode(code);
        }
        else
        {
            Playfab.Login();
        }
    }

    public void Reset()
    {
        startPosition = -1;
        userInfosAllTab.Clear();
        userInfosWorldTab.Clear();
        userInfosCountryTab.Clear();
        UpdateScore(() =>
        {
            Playfab.GetPlayerProfile(result =>
            {
                GetMoreLeaderboard(() =>
                {
                    GetUserInfoByDisplayName(result.PlayerProfile.DisplayName);
                    GetUserInfo();
                });
            });
        });
    }

    public void Login(string userName, string countryCode, Action callbackResult, Action callbackError)
    {
        Playfab.Login(
            (result) =>
            {
                Playfab.UpdateDisplayName(
                    userName + "|" + countryCode,
                    (result) =>
                    {
                        UpdateScore(() =>
                        {
                            Playfab.GetPlayerProfile(
                                (result) =>
                                {
                                    Data.PlayerId = result.PlayerProfile.PlayerId;
                                    DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                                    {
                                        GetMoreLeaderboard(() =>
                                        {
                                            GetUserInfoByDisplayName(result.PlayerProfile.DisplayName);
                                            GetUserInfo();
                                            callbackResult();
                                            Show();
                                        });
                                    });
                                }
                            );
                        });
                    },
                    (error) =>
                    {
                        callbackError();
                    }
                );
            }
        );
    }

    public void GetUserInfo()
    {
        GetUserInfoTab();
        GetUserInfoCurrent();
    }

    public void GetUserInfoTab()
    {
        userInfosWorldTab = userInfosAllTab;
        int index = 1;
        userInfosWorldTab.ForEach(userInfo => { userInfo.IndexWorld = index; index++; });

        userInfosCountryTab = userInfosAllTab.FindAll(userInfo => userInfo.CountryCode == UserInfoCurrent.CountryCode);
        index = 1;
        userInfosCountryTab.ForEach(userInfo => { userInfo.IndexCountry = index; index++; });
    }

    public void GetUserInfoCurrent()
    {
        UserInfoCurrent = UserInfos.Find(userInfo => userInfo.PlayerId == Data.PlayerId);
    }

    public void GetUserInfoByDisplayName(string displayName)
    {
        string[] split = displayName.Split('|');
        UserInfoCurrent.Name = split[0];
        UserInfoCurrent.CountryCode = split[1];
    }

    public void Show()
    {
        if (Data.PlayerId == "")
        {
            PopupController.Instance.Show<LeaderboardLoginPopup>();
        }
        else
        {
            PopupController.Instance.Show<LeaderboardPopup>();
        }
    }

    public void GetMoreLeaderboard(Action action = null)
    {
        startPosition++;
        Playfab.GetLeaderboard(
            startPosition * Playfab.MaxResultsCount,
            result =>
            {
                result.Leaderboard.ForEach(entry =>
                {
                    string[] split = entry.DisplayName.Split('|');
                    userInfosAllTab.Add(new LeaderboardUserInfo
                    {
                        Sprite = ResourcesController.Country.GetDataByCode(split[1]).Sprite,
                        Name = split[0],
                        CountryCode = split[1],
                        PlayerId = entry.Profile.PlayerId,
                        Stat = entry.StatValue + 1
                    });
                });

                action?.Invoke();
            }
        );
    }

    public void UpdateScore(Action action = null)
    {
        Playfab.UpdateScore(
            Data.CurrentLevel,
            result => action?.Invoke()
        );
    }
}

public class LeaderboardUserInfo
{
    public Sprite Sprite;
    public string PlayerId;
    public string Name;
    public int Stat;
    public string CountryCode;
    public int IndexWorld;
    public int IndexCountry;
    public int Index => LeaderboardController.Instance.IsWorldTab ? IndexWorld : IndexCountry;
    public string Rank => Index > Playfab.MaxResultsCount ? $"{textTab}: +{Playfab.MaxResultsCount}" : $"{textTab}: {Index}";
    private string textTab => LeaderboardController.Instance.IsWorldTab ? "World rank" : "Country rank";
}
