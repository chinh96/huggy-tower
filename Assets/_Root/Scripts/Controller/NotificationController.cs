using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    public string Id;
    public string Name;
    public string Description;
    public string Title;
    public string Text;
    public int Days;

    private void Start()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = this.Id,
            Name = Name,
            Importance = Importance.Default,
            Description = Description,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = Title;
        notification.Text = Text;
        notification.FireTime = System.DateTime.Now.AddDays(Days);
        AndroidNotificationCenter.SendNotification(notification, Id);
#elif UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(Days, 0, 0, 0, 0),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            Identifier = Id,
            Title = Title,
            Body = Text,
            Subtitle = "",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }
}
