using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownBeforeNewDay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        InvokeRepeating("Countdown", 0, 1);
    }

    private void Countdown()
    {
        countdownText.text = Util.SecondsToTimeFormatBeforeNewDay();
    }
}