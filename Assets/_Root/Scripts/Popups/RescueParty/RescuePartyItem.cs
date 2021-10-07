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

    [SerializeField] private GameObject iconDone;

    private SkinData data;
    private RescuePartyPopup rescuePartyPopup;

    public void Init(SkinData data, RescuePartyPopup rescuePartyPopup)
    {
        this.data = data;
        this.rescuePartyPopup = rescuePartyPopup;
    }

    public void Reset()
    {
        if (data.IsUnlocked)
        {
            progress.fillAmount = 1;
            text.text = $"{data.NumberMedalTarget}/{data.NumberMedalTarget}";

        }
        else
        {
            progress.fillAmount = (float)Data.TotalGoldMedal / data.NumberMedalTarget;
            text.text = $"{Data.TotalGoldMedal}/{data.NumberMedalTarget}";
        }
        if (Type == RescuePartyType.Top100)
        {
            claimActiveButton.SetActive(false);
            claimDisableButton.SetActive(true);
            iconDone.SetActive(false);
            if (data.IsUnlocked)
            {
                claimActiveButton.SetActive(false);
                claimDisableButton.SetActive(false);
                iconDone.SetActive(true);
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
            iconDone.SetActive(data.IsUnlocked);
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
        Data.TotalGoldMedal -= data.NumberMedalTarget;

        rescuePartyPopup.Reset();

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
