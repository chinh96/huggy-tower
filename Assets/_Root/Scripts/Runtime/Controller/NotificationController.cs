using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    public string Id;
    public string Name;
    public string Description;
    public string Title;
    public string Text;
    public int Minutes;

    private void Start()
    {
#if UNITY_ANDROID
        if (Data.IdChannel == 0)
        {
            DontDestroyOnLoad(gameObject);
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
            notification.FireTime = System.DateTime.Now.AddSeconds(Minutes);

            Data.IdChannel = AndroidNotificationCenter.SendNotification(notification, Id);
        }
#endif
    }
}
