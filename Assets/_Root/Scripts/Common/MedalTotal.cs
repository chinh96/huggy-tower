using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MedalTotal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMedalTotal;

    private void Awake()
    {
        EventController.MedalTotalChanged += UpdateMedalText;
        UpdateMedalText();
    }

    private void UpdateMedalText()
    {
        textMedalTotal.text = Data.TotalGoldMedal.ToString();
    }

    public void ShowRescuePartyPopup()
    {
        PopupController.Instance.Show<RescuePartyPopup>(null, ShowAction.DoNothing);
    }
}
