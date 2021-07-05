using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;
using Lance.Common.LocalNotification;

public class NotificationController : Singleton<NotificationController>
{
    [SerializeField] private NotificationConsole notificationConsole;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CheckDailyQuestRepeat()
    {
        int timeSchedule = (int)Util.TimeBeforeNewDay().TotalSeconds;
        notificationConsole.UpdateDeliveryTimeBy("daily_quest_repeat", timeSchedule);
    }

    public void CheckDailyRewardRepeat()
    {
        int timeSchedule = -1;
        int totalDays = Data.TotalDays;
        List<int> dailyRewardsSkin = ResourcesController.DailyReward.DailyRewardsSkin;
        for (int i = dailyRewardsSkin.Count - 1; i >= 0; i--)
        {
            if (totalDays <= dailyRewardsSkin[i])
            {
                timeSchedule = (int)Util.TimeBeforeNewDay().TotalSeconds + (dailyRewardsSkin[i] - totalDays) * 1440;
            }
        }

        if (timeSchedule > -1)
        {
            notificationConsole.UpdateDeliveryTimeBy("daily_reward_repeat", timeSchedule);
        }
    }
}
