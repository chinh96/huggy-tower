using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RescuePartyItem : MonoBehaviour
{
    public RescuePartyType Type;
    [SerializeField] private GameObject claimActiveButton;
    [SerializeField] private GameObject claimDisableButton;
    [SerializeField] private Image progress;
    [SerializeField] private TextMeshProUGUI text;

    private SkinData data;

    public void Init(SkinData data)
    {
        this.data = data;
    }

    public void Reset()
    {
        text.text = $"{Data.TotalGoldMedal}/{data.NumberMedalTarget}";
        progress.fillAmount = (float)Data.TotalGoldMedal / data.NumberMedalTarget;
        if (Type == RescuePartyType.Top100)
        {
            claimActiveButton.SetActive(false);
            claimDisableButton.SetActive(true);
            if (data.IsUnlocked)
            {
                claimActiveButton.SetActive(false);
                claimDisableButton.SetActive(false);
            }
            else if ((DateTime.Now - DateTime.Parse(Data.DateTimeStartRescueParty)).TotalDays > 5)
            {
                LeaderboardRescuePartyController.Instance.IsTop100(() =>
                {
                    claimActiveButton.SetActive(true);
                    claimDisableButton.SetActive(false);
                });
            }
        }
        else
        {
            claimActiveButton.SetActive(Data.TotalGoldMedal >= data.NumberMedalTarget && !data.IsUnlocked);
            claimDisableButton.SetActive(Data.TotalGoldMedal < data.NumberMedalTarget && !data.IsUnlocked);
        }
    }

    public void OnClickClaim()
    {
        if (Type == RescuePartyType.Princess)
        {
            Data.CurrentSkinPrincess = data.SkinName;
        }
        else
        {
            Data.CurrentSkinHero = data.SkinName;
        }
        data.IsUnlocked = true;
        Reset();
        EventController.MedalTotalChanged?.Invoke();

        if (!Data.ClaimFirstSkinHalloween)
        {
            Data.ClaimFirstSkinHalloween = true;
            AnalyticController.ClaimFirstSkinHalloween();
        }

        if (Type == RescuePartyType.Top100)
        {
            AnalyticController.ClaimSkinTop100Halloween();
        }
    }
}
