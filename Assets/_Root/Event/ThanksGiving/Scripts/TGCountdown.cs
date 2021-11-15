using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TGCountdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCountdown;

    private void OnEnable()
    {
        InvokeRepeating("Countdown", 0, 1);
    }

    public void Countdown()
    {
        textCountdown.text = Util.FormatTime(TGDatas.TimeToTG);
    }

    private void OnDisable()
    {
        CancelInvoke("Countdown");
    }
}