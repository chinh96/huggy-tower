using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNoti : MonoBehaviour
{
    [SerializeField] private NotiType notiType;
    [SerializeField] private GameObject noti;

    private void Start()
    {
        EventController.CoinTotalChanged += CheckNoti;
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
                hasNoti = Resource.Universe.HasNotiUniverse;
                break;
            case NotiType.World:
                hasNoti = Resource.Universe.HasNotiWorld;
                break;
            case NotiType.Build:
                hasNoti = Resource.Universe.HasNotiBuild;
                break;
            case NotiType.Skin:
                hasNoti = Resource.Hero.HasNoti;
                break;
            case NotiType.Daily:
                hasNoti = Resource.DailyReward.HasNoti;
                break;
        }

        noti.SetActive(hasNoti);
    }

    private void OnDestroy()
    {
        EventController.CoinTotalChanged -= CheckNoti;
    }
}

public enum NotiType
{
    Universe,
    World,
    Build,
    Skin,
    Daily
}
