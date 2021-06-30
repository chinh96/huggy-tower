using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject buttonActive;
    [SerializeField] private GameObject buttonDeactive;
    [SerializeField] private TextMeshProUGUI bonus;
    [SerializeField] private Image progress;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject done;

    private AchievementData data;

    public void Init(AchievementData data)
    {
        this.data = data;

        Reset();
    }

    public void Reset()
    {
        image.sprite = data.Sprite;
        number.text = data.NumberCurrent + "/" + data.NumberTarget;
        title.text = data.Text.Replace("{}", data.NumberTarget.ToString());
        buttonActive.SetActive(data.NumberCurrent >= data.NumberTarget);
        buttonDeactive.SetActive(data.NumberCurrent < data.NumberTarget);
        bonus.text = data.NumberTarget.ToString();
        progress.fillAmount = (float)data.NumberCurrent / data.NumberTarget;
        button.SetActive(data.NumberCurrent < data.NumberTarget);
        done.SetActive(data.NumberCurrent >= data.NumberTarget);
    }

    public void OnClickClaimButton()
    {
        Data.CoinTotal += data.NumberTarget;
        Reset();
    }
}
