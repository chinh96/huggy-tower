using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        title.text = item.Title;
        bonus.text = item.Bonus.ToString();
        buttonActive.SetActive(item.IsUnlocked && !item.IsClaimed);
        buttonDeactive.SetActive(!item.IsUnlocked);
        button.SetActive(!item.IsClaimed);
        done.SetActive(item.IsClaimed);
        progress.fillAmount = (float)item.NumberCurrent / item.NumberTarget;
        number.text = item.Number;
    }

    public void OnClickClaimButton()
    {
        AnalyticController.ClaimDailyQuest();
        dailyQuestPopup.GenerateCoin(buttonActive, item.Bonus);
        item.IsClaimed = true;
        Reset();
    }
}
