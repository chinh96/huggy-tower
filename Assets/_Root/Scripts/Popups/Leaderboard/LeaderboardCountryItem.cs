using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardCountryItem : MonoBehaviour
{
    [SerializeField] private Image flag;
    [SerializeField] private TextMeshProUGUI countryName;

    private LeaderboardLoginPopup leaderboardLoginPopup;
    private int index;
    private CountryData countryData;

    public void Init(CountryData countryData, LeaderboardLoginPopup leaderboardLoginPopup = null)
    {
        this.countryData = countryData;
        this.leaderboardLoginPopup = leaderboardLoginPopup;

        flag.sprite = countryData.Sprite;
        countryName.text = countryData.Name;
    }

    public void OnClick()
    {
        leaderboardLoginPopup.ChangeCurrent(countryData);
    }
}
