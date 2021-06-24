using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardNetwork : Popup
{
    [SerializeField] private TextMeshProUGUI text;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        if (Util.NotInternet)
        {
            text.text = "Check your network connection again!";
        }
        else
        {
            text.text = "Connecting...";
        }
    }
}
