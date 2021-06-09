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
        }

        noti.SetActive(hasNoti);
    }

    private void OnDestroy()
    {
        EventController.CoinTotalChanged -= CheckNoti;
    }
}
