using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Purchasing;

public class HomeController : Singleton<HomeController>
{
    [SerializeField] private GameObject rescuePartyButton;
    [SerializeField] private GameObject fbLoginButton;
    [SerializeField] private GameObject removeAdsButton;
    [SerializeField] private Image overlay;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject backgroundHalloween;
    [SerializeField] private GameObject debugButton;

    [SerializeField] private GameObject backgroundNormal;
    [SerializeField] private GameObject backgroundWorld;

    public GameObject BackgroundTG;
    public void OnClickDebugButton()
    {
        PopupController.Instance.Show<DebugPopup>(null, ShowAction.DoNothing);
    }

    protected override void Awake()
    {
        base.Awake();

        CheckButton();
        overlay.DOFade(0, 0);

    }

    public void ResetBackground(){
        
        // int childs = backgroundWorld.transform.childCount;
        // for (int i = 0; i < childs; i++)
        // {
        //     Destroy(backgroundWorld.transform.GetChild(i).gameObject);
        // }
        backgroundNormal.GetComponent<Image>().enabled = false;
        var room = PopupController.Instance.GetPopup<RoomPopup>().gameObject;
        //Instantiate(room.GetComponent<RoomPopup>().GetRoomCurrentObject(), backgroundWorld.transform);
        room.GetComponent<RoomPopup>().GetRoomCurrentObject().GetComponent<Room>().SetBackground();
        room.GetComponent<RoomPopup>().GetRoomCurrentObject().transform.SetParent(backgroundWorld.transform, false);
    }

    public void OnPurchaseSuccessRemoveAds()
    {
        // Data.IsRemovedAds = true;
        // CheckButton();
        //
        // AnalyticController.AdjustLogEventPurchaseItem("o6ssbb", 2.99f, "USD", product.transactionID);
        IAPManager.Instance.PurchaseProduct(Constants.IAP_REMOVE_ADS);
    }

    public void CheckButton()
    {
#if UNITY_IOS
        fbLoginButton.SetActive(RemoteConfigController.Instance.EnableFbLogin);
#endif
        //removeAdsButton.SetActive(!Data.IsRemovedAds);
        rescuePartyButton.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0);
        debugButton.SetActive(ResourcesController.Config.EnableTest);
    }

    private void Start()
    {
        canvasScaler.matchWidthOrHeight = Camera.main.aspect > .7f ? 1 : 0;
        SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
        FadeOutOverlay();
        CheckNewUpdatePopup();
        // CheckAchievementDailyQuest();
        // CheckRescueParty();
        // CheckTG();
        // checkAllEvent();
        //AdController.Instance.Request();
        AdController.Instance.HideBanner();

        //ResetBackground();
        // CheckLanguage();
    }

    private void CheckAchievementDailyQuest()
    {
        NotificationController.Instance.CheckDailyQuestRepeat();
        NotificationController.Instance.CheckDailyRewardRepeat();

        ResourcesController.Achievement.IncreaseByType(AchievementType.PlayToLevel);

        ResourcesController.Achievement.CheckCompleteCastle();
        ResourcesController.DailyQuest.CheckCompleteCastle();
    }

    private void CheckTG()
    {
        BackgroundTG.SetActive(TGDatas.IsInTG);
    }

    // if the player in top100 => display popup
    private void CheckRescueParty()
    {
        backgroundHalloween.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0);
        ResourcesController.ReceiveSkinRescueParty(() =>
        {
            PopupController.Instance.Show<RescuePartyReceiveSkinPopup>();
        });
    }

    private void CheckLanguage()
    {
        if (Data.FirstOpenLanguage)
        {
            Data.FirstOpenLanguage = false;
            PopupController.Instance.Show<PopupSelectLanguage>();
        }
    }

    public void TapToStart()
    {
        SoundController.Instance.PlayOnce(SoundType.ButtonStart);
        overlay.gameObject.SetActive(true);

        PopupController.Instance.GetPopup<RoomPopup>().gameObject.GetComponent<RoomPopup>().ReturnCurrentRoomToOriginalPosition();
        
        FadeInOverlay(() =>
        {
            SceneManager.LoadSceneAsync(Constants.GAME_SCENE);
        });
    }

    private void FadeInOverlay(Action action = null)
    {
        overlay.gameObject.SetActive(true);
        overlay.DOFade(1, .3f).OnComplete(() =>
        {
            action?.Invoke();
        });
    }

    private void FadeOutOverlay()
    {
        overlay.DOFade(0, .3f).OnComplete(() =>
        {
            overlay.gameObject.SetActive(false);
        });
    }

    public void OnClickDailyButton()
    {
        AnalyticController.ClickDailyReward();
        PopupController.Instance.Show<DailyRewardPopup>();
    }

    public void OnClickCastleButton()
    {
        //PopupController.Instance.Show<WorldPopup>();
        PopupController.Instance.Show<RoomPopup>();
    }

    public void OnClickSkinButton()
    {
        PopupController.Instance.Show<SkinPopup>();
    }

    public void OnClickAchievementButton()
    {
        PopupController.Instance.Show<AchievementPopup>();
    }

    public void OnClickSettingButton()
    {
        PopupController.Instance.Show<SettingPopup>();
    }

    public void OnClickFacebookButton()
    {
        PopupController.Instance.Show<FacebookPopup>();
    }

    public void OnClickDailyQuestButton()
    {
        PopupController.Instance.Show<DailyQuestPopup>();
    }

    public void OnClickAchievementDailyQuestButton()
    {
        PopupController.Instance.Show<AchievementDailyQuestPopup>();
    }

    public void OnClickLibraryButton()
    {
        PopupController.Instance.Show<LibraryPopup>();
    }

    public void OnClickLeaderboardButton()
    {
        AnalyticController.ClickRankButton();
        LeaderboardController.Instance.Show();
    }

    public void OnClickLeaderboardRescuePartyButton()
    {
        LeaderboardRescuePartyController.Instance.Show();
    }

    public void OnClickRescuePartyButton()
    {
        Data.FirstOpenRescuePartyInHome = false;
        PopupController.Instance.Show<RescuePartyPopup>();
    }

    public void CheckNewUpdatePopup()
    {
        if (RemoteConfigController.Instance.HasNewUpdate && !Data.DontShowUpdateAgain)
        {
            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                PopupController.Instance.Show<NewUpdatePopup>();
            });
        }
    }

    public void OnClickHalloweenButton()
    {
        AnalyticController.ClickButtonHalloween();
        PopupController.Instance.Show<CoomingSoonPopup>();
    }

    public void OnClickDailyEvent()
    {
        PopupController.Instance.Show<DailyRewardPopupEvent>();

    }
    private void checkAllEvent()
    {
        // var haveTime = Util.GetStateItemDaily(Data.DailyRewardEventCurrent, Data.DailyRewardEventCurrent);
        var data = ResourcesController.DailyEventReward.EventCollectRewards;
        for (int i = 0; i < data.Count; i++)
        {
            var itemData = data[i];
            checkHaveItem(itemData);
        }
    }
    
    // the player doesn't need to claim the skin but still has it.
    private void checkHaveItem(ItemConfigCollectEvent item)
    {
        for (int i = 0; i < TGDatas.ClaimedItems.Length; i++)
        {
            var id = TGDatas.ClaimedItems[i];
            if (item.Id == id)
            {
                if (!item.isSkinPrincess)
                {
                    var dataSkin = ResourcesController.Hero.SkinDatas[item.SkinId];
                    if (!dataSkin.IsUnlocked)
                        dataSkin.IsUnlocked = true;
                }
                else
                {
                    var dataSkin = ResourcesController.Princess.SkinDatas[item.SkinId];
                    if (!dataSkin.IsUnlocked)
                        dataSkin.IsUnlocked = true;
                }
            }
        }
    }
}