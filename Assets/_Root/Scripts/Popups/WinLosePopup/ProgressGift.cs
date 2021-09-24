using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class ProgressGift : MonoBehaviour
{
    [SerializeField] private Image progress;
    [SerializeField] private float duration;
    [SerializeField] private TextMeshProUGUI text;

    public void Reset()
    {
        progress.fillAmount = Data.PercentProgressGift / 100f;
        text.text = $"{Data.PercentProgressGift}%";
    }

    public void Move(Action action)
    {
        int oldValue = Data.PercentProgressGift;
        int newValue = Data.PercentProgressGift + ResourcesController.Config.PercentProgressGiftBonused;
        if (newValue > 100)
        {
            newValue = 100;
        }

        Data.PercentProgressGift = newValue;

        DOTween.To(() => oldValue, x =>
        {
            oldValue = x;
            text.text = $"{oldValue}%";
        }, newValue, duration);

        progress.DOFillAmount(Data.PercentProgressGift / 100f, duration).OnComplete(() =>
        {
            action?.Invoke();
            if (Data.PercentProgressGift >= 100)
            {
                List<SkinData> skinsLocked = ResourcesController.Hero.SkinsCoin;
                if (skinsLocked.Count > 0)
                {
                    Data.SkinGift = skinsLocked[UnityEngine.Random.Range(0, skinsLocked.Count)];
                    PopupController.Instance.Show<GiftPopup>(null, ShowAction.DoNothing);
                }
                Data.PercentProgressGift = 0;
            }
        });
    }
}
