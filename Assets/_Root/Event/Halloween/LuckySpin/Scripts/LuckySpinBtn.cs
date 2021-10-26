using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckySpinBtn : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0);
    }

    public void OnClick()
    {
        AnalyticController.LogEvent(LuckySpinConstants.CLICK_LUCKY_SPIN, new Firebase.Analytics.Parameter[] { });
        PopupController.Instance.Show<LuckySpinPopup>();
    }
}
