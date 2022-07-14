using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;

public class LeaderboardPopup : Popup
{
    [SerializeField] private GameObject bg1;
    [SerializeField] private GameObject bg2;
    [SerializeField] private LeaderboardTab worldTab;
    [SerializeField] private LeaderboardTab countryTab;
    //[SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private List<LeaderboardItem> leaderboardItems;
    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject content;

    private int page = 0;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        LeaderboardData.IsWorldTab = true;
        LeaderboardData.UserInfos.Clear();
        Reset();
    }

    private void Reset()
    {
        if (Util.NotInternet)
        {
            bg1.SetActive(false);
            bg2.SetActive(true);
        }
        else
        {
            bg1.SetActive(true);
            bg2.SetActive(false);
            loading.SetActive(true);
            content.SetActive(false);
            LeaderboardController.Instance.Reset(() =>
            {
                loading.SetActive(false);
                content.SetActive(true);
                ResetContent();
            });
        }
    }

    private void ResetContent()
    {
        page = 0;

        LeaderboardController.Instance.GetUserInfoCurrent();
        //name.text = LeaderboardData.UserInfoCurrent.Name;

        //string textTab = LeaderboardData.IsWorldTab ? "World rank" : "Country rank";
        string term = LeaderboardData.IsWorldTab ? "LeaderBoard_txtWorldRank rank" : "LeaderBoard_txtCountryRank";
        rank.GetComponent<Localize>().SetTerm(term);
        //string textRank = LeaderboardData.UserInfoCurrent.Index > Playfab.MaxResultsCount ? $"{textTab}: +{Playfab.MaxResultsCount}" : $"{textTab}: {LeaderboardData.UserInfoCurrent.Index}";
        string value = LeaderboardData.UserInfoCurrent.Index > Playfab.MaxResultsCount ? "+ " + Playfab.MaxResultsCount : LeaderboardData.UserInfoCurrent.Index.ToString();
        //rank.text = textRank;
        rank.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", value, true);
        FillData();
    }

    private void ResetButton()
    {
        previousButton.SetActive(page > 0);
        nextButton.SetActive((page + 1) * 10 < LeaderboardData.UserInfos.Count);
    }

    private void FillData()
    {
        int offset = 0;
        List<LeaderboardUserInfo> userInfoByTab = LeaderboardData.UserInfos;
        leaderboardItems.ForEach(item =>
        {
            item.gameObject.SetActive(false);
            int index = page * 10 + offset;
            if (index < userInfoByTab.Count)
            {
                var userInfo = userInfoByTab[index];
                item.Init(userInfo);
                item.gameObject.SetActive(true);
            }
            else
            {
                nextButton.SetActive(false);
            }
            offset++;
        });

        ResetButton();

        if ((page + 1) * 10 >= LeaderboardData.UserInfos.Count)
        {
            LeaderboardController.Instance.GetMoreLeaderboard(() => ResetButton(), false);
        }
    }

    public void OnClickWorldTab()
    {
        if (!LeaderboardData.IsWorldTab)
        {
            LeaderboardData.IsWorldTab = true;
            worldTab.SetActive(true);
            countryTab.SetActive(false);

            Reset();
        }
    }

    public void OnClickCountryTab()
    {
        if (LeaderboardData.IsWorldTab)
        {
            LeaderboardData.IsWorldTab = false;
            worldTab.SetActive(false);
            countryTab.SetActive(true);

            Reset();
        }
    }

    public void OnClickPreviousButton()
    {
        page--;
        FillData();
    }

    public void OnClickNextButton()
    {
        page++;
        FillData();
    }
}
