using System;
using UnityEngine;

public class ButtonNoti : MonoBehaviour
{
    [SerializeField] private NotiType notiType;
    [SerializeField] private GameObject noti;

    private void Start()
    {
        EventController.CoinTotalChanged += CheckNoti;
        EventController.LoginLeaderBoard += CheckNoti;
        EventController.MedalTotalChanged += CheckNoti;
        EventController.LuckySpinChanged += CheckNoti;
        EventController.DailyEventClaim += CheckNoti;
    }

    private void OnEnable()
    {
        CheckNoti();
    }

    public void CheckNoti()
    {
        bool hasNoti = false;

        switch (notiType)
        {
            case NotiType.Universe:
                hasNoti = ResourcesController.Universe.HasNotiUniverse;
                break;
            case NotiType.World:
                hasNoti = ResourcesController.Universe.HasNotiWorld;
                break;
            case NotiType.Build:
                hasNoti = ResourcesController.Universe.HasNotiBuild;
                break;
            case NotiType.Skin:
                hasNoti = ResourcesController.Hero.HasNoti;
                break;
            case NotiType.Daily:
                hasNoti = ResourcesController.DailyReward.HasNoti;
                break;
            case NotiType.Achievement:
                hasNoti = ResourcesController.Achievement.HasNoti;
                break;
            case NotiType.DailyQuest:
                hasNoti = ResourcesController.DailyQuest.HasNoti;
                break;
            case NotiType.Facebook:
                hasNoti = Data.JoinFbProgress < 2;
                break;
            case NotiType.Leaderboard:
                hasNoti = Data.PlayerId == "";
                break;
            case NotiType.AchievementDailyQuest:
                hasNoti = ResourcesController.Achievement.HasNoti || ResourcesController.DailyQuest.HasNoti;
                break;
            case NotiType.RescueParty:
                if (HomeController.Instance != null && Data.FirstOpenRescuePartyInHome)
                {
                    hasNoti = true;
                }
                else if (GameController.Instance != null && Data.FirstOpenRescuePartyInGame)
                {
                    hasNoti = true;
                }
                else
                {
                    hasNoti = ResourcesController.SkinRescuePartys.Exists(data => data.HasNotiRescueParty);
                }
                break;
            case NotiType.LuckySpin:
                if (LuckySpinDatas.LuckySpinTimeStart == "")
                {
                    hasNoti = true;
                }
                else
                {
                    hasNoti = DateTime.Parse(LuckySpinDatas.LuckySpinTimeStart).AddMinutes(10) < DateTime.Now;
                }
                break;
            case NotiType.ThanksGiving:

                hasNoti = ResourcesController.SkinsTG.Exists(data => data.HasNotiTG);

                var haveTime = Util.GetStateItemDaily(Data.DailyRewardEventCurrent, Data.DailyRewardEventCurrent);
                if (haveTime == StateClaimDailyEvent.CAN_CLAIM)
                {
                    hasNoti = true;
                }
                // if (Data.CurrentLevel <= 5)
                // {
                //     hasNoti = false;
                // }

                break;

            case NotiType.ThanksGiving2:

                hasNoti = ResourcesController.SkinsTG.Exists(data => data.HasNotiTG);

                var haveTime2 = Util.GetStateItemDaily(Data.DailyRewardEventCurrent, Data.DailyRewardEventCurrent);
                if (haveTime2 == StateClaimDailyEvent.CAN_CLAIM)
                {
                    hasNoti = true;
                }
                break;

        }

        if (noti != null)
        {
            noti.SetActive(hasNoti);
        }
    }

    private void OnDestroy()
    {
        EventController.CoinTotalChanged -= CheckNoti;
        EventController.LoginLeaderBoard -= CheckNoti;
        EventController.MedalTotalChanged -= CheckNoti;
        EventController.LuckySpinChanged -= CheckNoti;
        EventController.DailyEventClaim -= CheckNoti;
    }
}
