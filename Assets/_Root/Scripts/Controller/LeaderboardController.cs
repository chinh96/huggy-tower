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
    [NonSerialized] public List<LeaderboardUserInfo> UserInfos = new List<LeaderboardUserInfo>();

    private int startPosition = -1;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Reset(Action action = null)
    {
        startPosition = -1;
        UserInfos.Clear();

        if (UserInfoCurrent.Name == "")
        {
            LoginFirst(action);
        }
        else
        {
            UpdateScore(() =>
            {
                GetMoreLeaderboard(() =>
                {
                    action?.Invoke();
                });
            });
        }
    }

    public void LoginFirst(Action action = null)
    {
        Playfab.Login(result =>
        {
            Playfab.GetPlayerProfile(result =>
            {
                string[] split = result.PlayerProfile.DisplayName.Split('|');
                UserInfoCurrent.Name = split[0];
                UserInfoCurrent.CountryCode = split[1];

                UpdateScore(() =>
                {
                    GetMoreLeaderboard(() =>
                    {
                        GetUserInfoCurrent();
                        action?.Invoke();
                    });
                });
            });
        });
    }

    public void Login(string userName, string countryCode, Action callbackResult = null, Action callbackError = null, Action callbackErrorNetwork = null)
    {
        Playfab.Login(
            (result) =>
            {
                Playfab.UpdateDisplayName(
                    userName + "|" + countryCode,
                    (result) =>
                    {
                        UserInfoCurrent.Name = userName;
                        UserInfoCurrent.CountryCode = countryCode;

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
                                            GetUserInfoCurrent();
                                            callbackResult?.Invoke();
                                            Show();
                                        });
                                    });
                                }
                            );
                        });
                    },
                    (error) =>
                    {
                        callbackError?.Invoke();
                    }
                );
            },
            (error) =>
            {
                callbackErrorNetwork?.Invoke();
            }
        );
    }

    public void GetUserInfoCurrent()
    {
        var userInfo = UserInfos.Find(userInfo => userInfo.PlayerId == Data.PlayerId);
        if (userInfo == null)
        {
            UserInfoCurrent.Index = Playfab.MaxResultsCount + 1;
        }
        else
        {
            UserInfoCurrent.Index = userInfo.Index;
        }
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
            IsWorldTab ? "HERO_TOWER_WAR" : UserInfoCurrent.CountryCode,
            startPosition * Playfab.MaxResultsCount,
            result =>
            {
                result.Leaderboard.ForEach(
                    entry =>
                    {
                        string[] split = entry.DisplayName.Split('|');
                        this.UserInfos.Add(new LeaderboardUserInfo
                        {
                            Sprite = ResourcesController.Country.GetDataByCode(split[1]).Sprite,
                            Name = split[0],
                            CountryCode = split[1],
                            PlayerId = entry.Profile.PlayerId,
                            Stat = entry.StatValue + 1,
                            Index = entry.Position + 1
                        });
                    }
                );

                action?.Invoke();
            }
        );
    }

    public void UpdateScore(Action action = null)
    {
        Playfab.UpdateScore(
            Data.CurrentLevel,
            "HERO_TOWER_WAR"
        );

        Playfab.UpdateScore(
            Data.CurrentLevel,
            UserInfoCurrent.CountryCode,
            result => action?.Invoke()
        );
    }
}

public class LeaderboardUserInfo
{
    public Sprite Sprite;
    public string PlayerId;
    public string Name = "";
    public int Stat;
    public string CountryCode;
    public int Index;
    public string Rank => Index > Playfab.MaxResultsCount ? $"{textTab}: +{Playfab.MaxResultsCount}" : $"{textTab}: {Index}";
    private string textTab => LeaderboardController.Instance.IsWorldTab ? "World rank" : "Country rank";
}
