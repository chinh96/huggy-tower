using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardPopup : Popup
{
    [SerializeField] private LeaderboardTab worldTab;
    [SerializeField] private LeaderboardTab countryTab;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private List<LeaderboardItem> leaderboardItems;

    private int page = 0;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        ResetContent();
    }

    private void ResetContent()
    {
        page = 0;

        LeaderboardController.Instance.GetUserInfo();
        name.text = LeaderboardController.Instance.UserInfoCurrent.Name;
        rank.text = LeaderboardController.Instance.UserInfoCurrent.Rank;

        FillData();
    }

    private void ResetButton()
    {
        previousButton.SetActive(page > 0);
        nextButton.SetActive((page + 1) * 10 < LeaderboardController.Instance.UserInfos.Count);
    }

    private void FillData()
    {
        int offset = 0;
        List<LeaderboardUserInfo> userInfoByTab = LeaderboardController.Instance.UserInfos;
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

        if ((page + 1) * 10 == LeaderboardController.Instance.UserInfos.Count)
        {
            LeaderboardController.Instance.GetMoreLeaderboard(() => ResetButton());
        }
    }

    public void OnClickWorldTab()
    {
        LeaderboardController.Instance.IsWorldTab = true;
        worldTab.SetActive(true);
        countryTab.SetActive(false);

        ResetContent();
    }

    public void OnClickCountryTab()
    {
        LeaderboardController.Instance.IsWorldTab = false;
        worldTab.SetActive(false);
        countryTab.SetActive(true);

        ResetContent();
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
