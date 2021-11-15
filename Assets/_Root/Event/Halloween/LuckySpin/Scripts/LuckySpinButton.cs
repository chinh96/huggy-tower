using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckySpinButton : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0);
        EventController.LuckySpinChanged?.Invoke();
    }

    public void OnClick()
    {
        AnalyticController.LogEvent(LuckySpinConstants.CLICK_LUCKY_SPIN);
        PopupController.Instance.Show<LuckySpinPopup>();
    }
}
