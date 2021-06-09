using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ProgressGift : MonoBehaviour
{
    [SerializeField] private Image progress;
    [SerializeField] private float duration;

    public void Reset()
    {
        progress.fillAmount = Data.PercentProgressGift / 100f;
    }

    public void Move(Action action)
    {
        Data.PercentProgressGift += ResourcesController.Config.PercentProgressGiftBonused;
        progress.DOFillAmount(Data.PercentProgressGift / 100f, duration).OnComplete(() =>
        {
            action?.Invoke();
            if (Data.PercentProgressGift >= 100)
            {
                List<SkinData> skinsLocked = ResourcesController.Hero.SkinsLocked;
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
