using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TGRankPopup : Popup
{
    [SerializeField] private GameObject bg1;
    [SerializeField] private GameObject bg2;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private List<TGRankItem> rankItems;
    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject content;

    private int page = 0;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        LeaderboardData.IsWorldTab = true;
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
            TGRankController.Instance.Reset(() =>
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

        TGRankController.Instance.GetUserInfoCurrent();
        name.text = LeaderboardData.UserInfoCurrent.Name;
        rank.text = LeaderboardData.UserInfoCurrent.Index > Playfab.MaxResultsCount ? $"Rank: +{Playfab.MaxResultsCount}" : $"Rank: {LeaderboardData.UserInfoCurrent.Index}";

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
        rankItems.ForEach(item =>
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
            TGRankController.Instance.GetMoreLeaderboard(() => ResetButton(), false);
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
