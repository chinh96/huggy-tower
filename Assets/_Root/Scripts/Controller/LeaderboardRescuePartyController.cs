using System;
using System.Globalization;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LeaderboardRescuePartyController : Singleton<LeaderboardRescuePartyController>
{
    private int startPosition = -1;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Reset(Action action = null)
    {
        startPosition = -1;
        LeaderboardData.UserInfos.Clear();

        if (LeaderboardData.UserInfoCurrent.Name == "")
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
                LeaderboardData.UserInfoCurrent.Name = split[0];
                LeaderboardData.UserInfoCurrent.CountryCode = split[1];

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
                        LeaderboardData.UserInfoCurrent.Name = userName;
                        LeaderboardData.UserInfoCurrent.CountryCode = countryCode;

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
        var userInfo = LeaderboardData.UserInfos.Find(userInfo => userInfo.PlayerId == Data.PlayerId);
        if (userInfo == null)
        {
            LeaderboardData.UserInfoCurrent.Index = Playfab.MaxResultsCount + 1;
        }
        else
        {
            LeaderboardData.UserInfoCurrent.Index = userInfo.Index;
        }
    }

    public void GetUserInfoByDisplayName(string displayName)
    {
        string[] split = displayName.Split('|');
        LeaderboardData.UserInfoCurrent.Name = split[0];
        LeaderboardData.UserInfoCurrent.CountryCode = split[1];
    }

    public void Show()
    {
        if (Data.PlayerId == "")
        {
            PopupController.Instance.Show<LeaderboardLoginPopup>(1, ShowAction.DoNothing);
        }
        else
        {
            PopupController.Instance.Show<LeaderboardRescuePartyPopup>(null, ShowAction.DoNothing);
        }
    }

    public void GetMoreLeaderboard(Action action = null, bool needCleaer = true)
    {
        startPosition++;
        if (needCleaer)
        {
            LeaderboardData.UserInfos.Clear();
        }
        Playfab.GetLeaderboard(
            "HALLOWEEN",
            startPosition * Playfab.MaxResultsCount,
            result =>
            {
                result.Leaderboard.ForEach(
                    entry =>
                    {
                        if (entry.DisplayName != null)
                        {
                            string[] split = entry.DisplayName.Split('|');
                            LeaderboardData.UserInfos.Add(new LeaderboardUserInfo
                            {
                                Sprite = ResourcesController.Country.GetDataByCode(split[1]).Sprite,
                                Name = split[0],
                                CountryCode = split[1],
                                PlayerId = entry.Profile.PlayerId,
                                Stat = entry.StatValue + 1,
                                Index = entry.Position + 1
                            });
                        }
                    }
                );

                action?.Invoke();
            }
        );
    }

    public void UpdateScore(Action action = null)
    {
        Playfab.UpdateScore(
            Data.TotalGoldMedal,
            "HALLOWEEN",
            result => action?.Invoke()
        );
    }

    public void IsTop100(Action action)
    {
        if (Data.PlayerId != "")
        {
            Reset(() =>
            {
                LeaderboardData.UserInfos.ForEach(userInfo =>
                {
                    if (userInfo.PlayerId == Data.PlayerId)
                    {
                        action?.Invoke();
                    }
                });
            });
        }
    }
}
