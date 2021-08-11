using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RescuePartyCountdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCountdown;

    private void OnEnable()
    {
        CancelInvoke("Countdown");
        InvokeRepeating("Countdown", 0, 1);
    }

    public void Countdown()
    {
        textCountdown.text = Util.FormatTime(Data.TimeToRescueParty);
    }
}
