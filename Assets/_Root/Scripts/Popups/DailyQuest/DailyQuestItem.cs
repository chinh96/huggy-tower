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

    private DailyQuestData data;
    private DailyQuestPopup dailyQuestPopup;

    public void Init(DailyQuestData data, DailyQuestPopup dailyQuestPopup)
    {
        this.data = data;
        this.dailyQuestPopup = dailyQuestPopup;
    }

    public void Reset()
    {
        image.sprite = data.Sprite;
        title.text = data.Title;
        bonus.text = data.Bonus.ToString();
        buttonActive.SetActive(data.IsUnlocked && !data.IsClaimed);
        buttonDeactive.SetActive(!data.IsUnlocked);
        button.SetActive(!data.IsClaimed);
        progress.fillAmount = (float)data.NumberCurrent / data.NumberTarget;
        number.text = (data.NumberCurrent > data.NumberTarget ? data.NumberTarget : data.NumberCurrent) + "/" + data.NumberTarget;
    }

    public void OnClickClaimButton()
    {
        AnalyticController.ClaimDailyQuest();
        dailyQuestPopup.GenerateCoin(buttonActive, data.Bonus);
        data.IsClaimed = true;
        Reset();
    }
}
