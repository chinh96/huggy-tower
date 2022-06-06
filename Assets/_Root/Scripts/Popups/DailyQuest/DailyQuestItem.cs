using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

public class DailyQuestItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI bonus;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject buttonActive;
    [SerializeField] private GameObject buttonDeactive;
    [SerializeField] private Image progress;
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private GameObject done;
    [SerializeField] private GameObject backgroundActive;

    private DailyQuestDayItem item;
    private DailyQuestPopup dailyQuestPopup;

    public void Init(DailyQuestDayItem item, DailyQuestPopup dailyQuestPopup)
    {
        this.item = item;
        this.dailyQuestPopup = dailyQuestPopup;
    }

    public void Reset()
    {
        image.sprite = item.Sprite;
        //title.text = item.Title;
        title.GetComponent<Localize>().SetTerm("DailyQuestItem_txt" + item.Type + "Type");
        title.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", item.NumberTarget.ToString(), true);

        bonus.text = item.Bonus.ToString();
        buttonActive.SetActive(item.IsUnlocked && !item.IsClaimed);
        buttonDeactive.SetActive(!item.IsUnlocked);
        button.SetActive(!item.IsClaimed);
        
        done.SetActive(item.IsClaimed);
        progress.fillAmount = (float)item.NumberCurrent / item.NumberTarget;
        number.text = item.Number;
        backgroundActive.SetActive(item.IsUnlocked);
    }

    public void OnClickClaimButton()
    {
        AnalyticController.ClaimDailyQuest();
        if (!Data.IsClaimFirstDailyQuest)
        {
            Data.IsClaimFirstDailyQuest = true;
            AnalyticController.ClaimFirstDailyQuest();
        }
        dailyQuestPopup.GenerateCoin(buttonActive, item.Bonus);
        item.IsClaimed = true;
        Reset();
    }
}
