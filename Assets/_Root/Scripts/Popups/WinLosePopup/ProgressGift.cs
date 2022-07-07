using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;
using Spine.Unity;

public class ProgressGift : MonoBehaviour
{
    // [SerializeField] private Image progress;
    [SerializeField] private float duration;
    // [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<GameObject> collectedCassettes;
    [SerializeField] private CoinGeneration coinGeneration;
    [SerializeField] private int giftCoin = 500;
    [SerializeField] private SkeletonGraphic gift;
    [SerializeField] private ParticleSystem collectFilm;

    public void Reset()
    {
        // progress.fillAmount = Data.PercentProgressGift / 100f;
        // text.text = $"{Data.PercentProgressGift}%";
        collectFilm.gameObject.SetActive(false);
        gift.Play("Idle", true);
        for (int i = 1; i <= 5; i++)
        {
            if (i <= Data.PercentProgressGift % 5) {
                collectedCassettes[i - 1].GetComponent<ScaleAnimation>().IsLoop = false;
                collectedCassettes[i - 1].SetActive(true);
            }
            else collectedCassettes[i - 1].SetActive(false);
        }
    }

    public void Move(Action action)
    {
        int oldValue = Data.PercentProgressGift % 5;

        Data.PercentProgressGift += ResourcesController.Config.PercentProgressGiftBonused;
        int newValue = Data.PercentProgressGift % 5;
        // if (newValue > 5)
        // {
        //     newValue = 5;
        // }
        if (newValue == 0) newValue = 5;

        collectFilm.gameObject.SetActive(false);
        collectFilm.transform.position = collectedCassettes[newValue - 1].transform.position;
        collectFilm.gameObject.SetActive(true);

        collectedCassettes[newValue - 1].SetActive(true);
        collectedCassettes[newValue - 1].GetComponent<ScaleAnimation>().IsLoop = true;
        // DOTween.To(() => oldValue, x =>
        // {
        //     oldValue = x;
        //     if (oldValue > 0){
        //         collectFilm.gameObject.SetActive(false);
        //         collectFilm.transform.position = collectedCassettes[oldValue - 1].transform.position;
        //         collectFilm.gameObject.SetActive(true);
        //         collectedCassettes[oldValue - 1].SetActive(true);
        //     }
        //     // text.text = $"{oldValue}%";
        // }, newValue, duration);

        // progress.DOFillAmount(Data.PercentProgressGift / 100f, duration).OnComplete(() =>
        action?.Invoke();
        if (Data.PercentProgressGift > 0)
        {
            if (Data.PercentProgressGift % 10 == 0)
            {
                gift.Play("Open", true);
                List<SkinData> skinsLocked = ResourcesController.Hero.SkinsCoin;
                if (skinsLocked.Count > 0)
                {
                    Data.SkinGift = skinsLocked[UnityEngine.Random.Range(0, skinsLocked.Count)];
                    PopupController.Instance.Show<GiftPopup>(null, ShowAction.DoNothing);
                }
            }
            else if (Data.PercentProgressGift % 5 == 0)
            {
                gift.Play("Open", false);
                int coinTotal = Data.CoinTotal + giftCoin;
                coinGeneration.GenerateCoin(() =>
                {
                    Data.CoinTotal++;
                }, () =>
                {
                    Data.CoinTotal = coinTotal;
                });
            }
            // Data.PercentProgressGift = 0;
        }
    }
}
