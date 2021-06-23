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

    public void GetUserInfoCurrent(string displayName = "")
    {
        if (displayName != "")
        {
            string[] split = displayName.Split('|');
            UserInfoCurrent.Name = split[0];
            UserInfoCurrent.CountryCode = split[1];
        }

        var index = UserInfos.FindIndex(userInfo => userInfo.PlayerId == Data.PlayerId);
        if (index == -1)
        {
            UserInfoCurrent.Stat = Data.CurrentLevel;
            UserInfoCurrent.Position = 101;
        }
        else
        {
            UserInfoCurrent.Stat = UserInfos[index].Stat;
            UserInfoCurrent.Position = index + 1;
        }
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
            Playfab.Login(result =>
            {
                Playfab.GetPlayerProfile(result =>
                {
                    GetMoreLeaderboard(() => GetUserInfoCurrent(result.PlayerProfile.DisplayName));
                });
            });
        }
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
                                            GetUserInfoCurrent(result.PlayerProfile.DisplayName);
                                            callbackResult();
                                            Show();
                                        });
                                    });
                                }
                            );
                        });
                    }
                );
            },
            (error) =>
            {
                callbackError();
            }
        );
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
                        Stat = entry.StatValue + 1,
                        Position = entry.Position + 1
                    });
                });

                userInfosWorldTab = userInfosAllTab;
                userInfosCountryTab = userInfosAllTab.FindAll(userInfo => userInfo.CountryCode == UserInfoCurrent.CountryCode);

                int index = 1;
                userInfosCountryTab.ForEach(userInfo => { userInfo.Position = index; index++; });

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
    public int Position;
    public string Rank => Position > 100 ? $"{textTab}: +100" : $"{textTab}: {Position}";
    private string textTab => LeaderboardController.Instance.IsWorldTab ? "World rank" : "Country rank";
}
