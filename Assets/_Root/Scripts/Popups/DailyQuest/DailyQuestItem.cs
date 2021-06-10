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
    }

    public void OnClickClaimButton()
    {
        dailyQuestPopup.GenerateCoin(buttonActive, data.Bonus);
        data.IsClaimed = true;
        Reset();
    }
}
