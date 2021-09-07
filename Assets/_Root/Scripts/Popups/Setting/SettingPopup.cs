using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;
using PlayFab;
using PlayFab.ClientModels;

public class SettingPopup : Popup
{
    [SerializeField] private GameObject purchaseButton;
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private GameObject updateButton;
    [SerializeField] private GameObject block;

    private void Awake() { purchaseButton.SetActive(false); }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        //versionText.text = $"Version {Application.version}";
        versionText.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", Application.version.ToString(), true);
#if UNITY_IOS
        purchaseButton.SetActive(true);
#endif
        updateButton.SetActive(RemoteConfigController.Instance.HasNewUpdate);
    }

    public void OnClickUpdateButton()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.gamee.herotowerwar");
#else
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1570840391");
#endif
    }

    public void OnClickRateUs() { RatingController.Instance.LinkToStore(); }

    public void OnClickShowPopupSelect() { PopupController.Instance.Show<PopupSelectLanguage>(); }

    #region connect

    private Action _actionCompleted;
    private Action _funcLogin;
    private bool _isBackup;

    private bool StatusLogin { get; set; }
    private bool CompletedRun { get; set; }

    private void ResetRun()
    {
        StatusLogin = false;
        CompletedRun = false;
    }

    private void LoginWithFacebook(string tokenId, Action actionCompleted, bool isBackup, Action funcLogin)
    {
        ResetRun();
        _actionCompleted = actionCompleted;
        _funcLogin = funcLogin;
        _isBackup = isBackup;
        Playfab.LoginWithFacebook(tokenId, OnLoginSuccessFacebook);
    }

    private void OnLoginSuccessFacebook(LoginResult result)
    {
        _actionCompleted?.Invoke();
        LoadUserData(result.PlayFabId);
    }

    private bool CheckConnection()
    {
#if UNITY_EDITOR
        return true;
#endif
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    private void SaveUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {
                        "data_part1",
                        $"{Data.DataVersion}_{Data.CurrentLevel}_{Data.CoinTotal}_{Data.TotalGoldMedal}_{Data.MaxLevel}_{Data.SoundState}_{Data.MusicState}_{Data.VibrateState}_{Data.IsRemovedAds}_{Data.IsVip}_{Data.DontShowUpdateAgain}_{(int)Data.WorldCurrent}_{LeaderboardData.UserInfoCurrent.Name}_{LeaderboardData.UserInfoCurrent.CountryCode}_{Data.CustomId}_{Data.PlayerId}_{Data.PercentProgressGift}"
                    },
                    {
                        "data_part3",
                        $"{ResourcesController.Hero.ConvertData()}_{ResourcesController.Princess.ConvertData()}_{ResourcesController.Universe.ConvertData()}_{ResourcesController.Achievement.ConvertData()}_{ResourcesController.Achievement.ConvertDataTarget()}"
                    },
                    {
                        "data_part4",
                        $"{Data.CurrentSkinHero}_{Data.CurrentSkinPrincess}_{Data.AchievementNumberCurrent}_{Data.DailyQuestNumberCurrent}_{Data.DailyRewardCurrent}_{Data.CountPlayLevel}"
                    }
                },
            },
            result => { Data.DataVersion++; },
            error =>
            {
                Debug.Log("ERROR:" + error.ErrorMessage);
            });
    }

    private void LoadUserData(string id)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest { PlayFabId = id, Keys = null },
            result =>
            {
                if (result.Data == null)
                {
                }
                else
                {
                    string title;
                    string titleLeft;
                    string titleRight;
                    string message;
                    if (_isBackup)
                    {
                        message = "Do you want to backup data\nfrom your device to server?\n\nData in server will be overrode";
                        title = "Backup Data";
                        titleLeft = "Device Data";
                        titleRight = "Server Data";
                    }
                    else
                    {
                        message = "Do you want to restore data\nfrom your server to device?\n\nData in device will be overrode";
                        title = "Restore Data";
                        titleLeft = "Server Data";
                        titleRight = "Device Data";
                    }

                    var dataVersion = -1;
                    string[] dataPart1 = new string[17];
                    if (result.Data.ContainsKey("data_part1"))
                    {
                        dataPart1 = result.Data["data_part1"].Value.Split('_');
                        dataVersion = int.Parse(dataPart1[0]);
                    }

                    if (dataVersion != -1)
                    {
                        int serverTotalSkin = 0;
                        int serverCurrentLevel = 0;
                        int serverCoin = 0;

                        if (result.Data.ContainsKey("data_part3"))
                        {
                            string[] part3 = result.Data["data_part3"].Value.Split('_');

                            string hero = part3[0];
                            string princess = part3[1];

                            if (!string.IsNullOrEmpty(hero))
                            {
                                var heroData = hero.Split('@');
                                var count = heroData.Length;
                                if (count > ResourcesController.Hero.SkinDatas.Count) count = ResourcesController.Hero.SkinDatas.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    var unlocked = bool.Parse(heroData[i]);
                                    ResourcesController.Hero.SkinDatas[i].IsUnlocked = unlocked;
                                    if (unlocked) serverTotalSkin++;
                                }
                            }

                            if (!string.IsNullOrEmpty(princess))
                            {
                                var princessData = princess.Split('@');
                                var count = princessData.Length;
                                if (count > ResourcesController.Princess.SkinDatas.Count) count = ResourcesController.Princess.SkinDatas.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    var unlocked = bool.Parse(princessData[i]);
                                    ResourcesController.Princess.SkinDatas[i].IsUnlocked = unlocked;
                                    if (unlocked) serverTotalSkin++;
                                }
                            }
                        }

                        if (result.Data.ContainsKey("data_part1"))
                        {
                            serverCurrentLevel = int.Parse(dataPart1[1]);
                            serverCoin = int.Parse(dataPart1[2]);
                        }

                        async void Do()
                        {
                            Data.DataVersion = dataVersion + 2;
                            if (result.Data.ContainsKey("data_part1"))
                            {
                                Data.CurrentLevel = int.Parse(dataPart1[1]);
                                Data.CoinTotal = int.Parse(dataPart1[2]);
                                Data.TotalGoldMedal = int.Parse(dataPart1[3]);
                                Data.MaxLevel = int.Parse(dataPart1[4]);
                                Data.SoundState = bool.Parse(dataPart1[5].ToLower());
                                Data.MusicState = bool.Parse(dataPart1[6].ToLower());
                                Data.VibrateState = bool.Parse(dataPart1[7].ToLower());
                                Data.IsRemovedAds = bool.Parse(dataPart1[8].ToLower());
                                Data.IsVip = bool.Parse(dataPart1[9].ToLower());
                                Data.DontShowUpdateAgain = bool.Parse(dataPart1[10].ToLower());
                                Data.WorldCurrent = (WorldType)int.Parse(dataPart1[11]);
                                LeaderboardData.UserInfoCurrent.Name = dataPart1[12];
                                LeaderboardData.UserInfoCurrent.CountryCode = dataPart1[13];
                                Data.CustomId = dataPart1[14];
                                Data.PlayerId = dataPart1[15];
                                Data.PercentProgressGift = int.Parse(dataPart1[16]);
                            }

                            if (result.Data.ContainsKey("data_part3"))
                            {
                                string[] dataPart3 = result.Data["data_part3"].Value.Split('_');
                                var universe = dataPart3[2];
                                var achievement = dataPart3[3];
                                var achievementTarget = dataPart3[4];
                                
                                ResourcesController.Universe.TransformData(universe);
                                ResourcesController.Achievement.TransformData(achievement);
                                ResourcesController.Achievement.TransformTargetData(achievementTarget);
                            }

                            if (result.Data.ContainsKey("data_part4"))
                            {
                                string[] dataPart4 = result.Data["data_part4"].Value.Split('_');

                                Data.CurrentSkinHero = dataPart4[0];
                                Data.CurrentSkinPrincess = dataPart4[1];
                                Data.AchievementNumberCurrent = int.Parse(dataPart4[2]);
                                Data.DailyQuestNumberCurrent = int.Parse(dataPart4[3]);
                                Data.DailyRewardCurrent = int.Parse(dataPart4[4]);
                                Data.CountPlayLevel = int.Parse(dataPart4[5]);
                            }


                            
                            SetActiveBlock(true);

                            var coins = GameObject.FindObjectsOfType<CoinTotal>();
                            for (int i = 0; i < coins.Length; i++)
                            {
                                coins[i].UpdateCoinText();
                            }
                            
                            var levels = GameObject.FindObjectsOfType<LevelText>();
                            for (int i = 0; i < levels.Length; i++)
                            {
                                levels[i].ChangeLevel();
                            }
                            
                            // re calculate list level
                            var go = await DataBridge.Instance.GetLevel(Data.CurrentLevel);
                            if (go.Item1 != null)
                            {
                                LevelMap levelMap = go.Item1.GetComponent<LevelMap>();

                                DataBridge.Instance.NextLevelLoaded = levelMap;
                                DataBridge.Instance.NextLevelLoaded.SetLevelLoaded(go.Item2, Data.CurrentLevel + 1);

                                DataBridge.Instance.PreviousLevelLoaded = levelMap;
                                DataBridge.Instance.PreviousLevelLoaded.SetLevelLoaded(go.Item2, Data.CurrentLevel + 1);
                            }

                            SetActiveBlock(false);
                        }

                        Action actionDo = Do;
                        Action actionSaveUserData = SaveUserData;
                        object[] obj =
                            {
                                actionDo, actionSaveUserData, message, title, serverCurrentLevel, serverCoin, serverTotalSkin, titleRight, titleLeft, _isBackup
                            };

                        PopupController.Instance.Show<BackupPopup>(obj);
                    }
                    else
                    {
                        SaveUserData();
                    }
                }
            },
            (error) => { Debug.Log(error.GenerateErrorReport()); });
    }

    private void ChangeLogin() { SetActiveBlock(false); }

    private void SetActiveBlock(bool state) { block.SetActive(state); }

    public void OnButtonRestoreDataPressed()
    {
        if (!CheckConnection())
        {
            PopupController.Instance.Show<NotificationPopup>("Please check your connection\nand try again late!");
            return;
        }

        if (FacebookController.Instance.IsLoggedIn)
        {
            SetActiveBlock(true);
            LoginWithFacebook(FacebookController.Instance.Token,
                ChangeLogin,
                false,
                () => { PopupController.Instance.Show<NotificationPopup>("Restore data success!"); });
        }
        else
        {
            SetActiveBlock(true);

            FacebookController.Instance.Login(() =>
                {
                    LoginWithFacebook(FacebookController.Instance.Token,
                        ChangeLogin,
                        false,
                        () => { PopupController.Instance.Show<NotificationPopup>("Restore data success!"); });
                },
                () =>
                {
                    SetActiveBlock(false);
                    PopupController.Instance.Show<NotificationPopup>("Error when login Facebook!\nPlease try again!");
                },
                () =>
                {
                    SetActiveBlock(false);
                    PopupController.Instance.Show<NotificationPopup>("Error when login Facebook!\nPlease try again!");
                });
        }
    }

    public void OnButtonBackupPressed()
    {
        if (!CheckConnection())
        {
            PopupController.Instance.Show<NotificationPopup>("Please check your connection\nand try again late!");
            return;
        }

        if (FacebookController.Instance.IsLoggedIn)
        {
            SetActiveBlock(true);
            LoginWithFacebook(FacebookController.Instance.Token, ChangeLogin, true, () => { PopupController.Instance.Show<NotificationPopup>("Backup data success!"); });
        }
        else
        {
            SetActiveBlock(true);

            FacebookController.Instance.Login(() =>
                {
                    LoginWithFacebook(FacebookController.Instance.Token,
                        ChangeLogin,
                        true,
                        () => { PopupController.Instance.Show<NotificationPopup>("Backup data success!"); });
                },
                () =>
                {
                    SetActiveBlock(false);
                    PopupController.Instance.Show<NotificationPopup>("Faild to login Facebook!\nPlease try again!");
                },
                () =>
                {
                    SetActiveBlock(false);
                    PopupController.Instance.Show<NotificationPopup>("Error when login Facebook!\nPlease try again!");
                });
        }
    }

    #endregion
}