using DG.Tweening;
using System;
using UnityEngine;
using TMPro;
using Spine.Unity;
using System.Collections.Generic;

public class LuckySpinPopup : Popup
{
    public GameObject SpinNowBtn;
    public GameObject CountdownBtn;
    public TextMeshProUGUI CountdownText;
    public SkeletonGraphic SpinSkeleton;
    public Transform Spin;
    public int SpinNumber = 10;
    public List<LuckySpinItem> LuckySpinItems;
    public float radian;
    public CoinGeneration CoinGeneration;
    public CoinGeneration CandyGeneration;
    public GameObject Overlay;

    private int SecondsRemaining;

    [ContextMenu("Execute")]
    public void Execute()
    {
        for (int i = 0; i < LuckySpinItems.Count; i++)
        {
            var item = LuckySpinItems[i];
            float angle = Mathf.PI * (3 - 2 * i) / 8;
            item.transform.localPosition = new Vector2(radian * Mathf.Cos(angle), radian * Mathf.Sin(angle));
            item.transform.eulerAngles = new Vector3(0, 0, -i * 45 - 22.5f);
            item.Setup();
        }
    }

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        if (LuckySpinDatas.LuckySpinTimeStart == "")
        {
            LuckySpinDatas.LuckySpinTimeStart = DateTime.Now.ToString();
        }
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        Reset();
        SpinSkeleton.Play("idle", true);
    }

    public void Reset()
    {
        SecondsRemaining = (int)(DateTime.Parse(LuckySpinDatas.LuckySpinTimeStart).AddMinutes(10) - DateTime.Now).TotalSeconds;
        if (SecondsRemaining > 0)
        {
            ShowSpinBtn(false);
            InvokeRepeating("Countdown", 0, 1);
        }
        else
        {
            ShowSpinBtn(true);
        }
    }

    private void ShowSpinBtn(bool state)
    {
        SpinNowBtn.SetActive(state);
        CountdownBtn.SetActive(!state);
    }

    public void Countdown()
    {
        if (SecondsRemaining > 0)
        {
            var distance = TimeSpan.FromSeconds(SecondsRemaining);
            CountdownText.text = string.Format("{0:00}:{1:00}", distance.Minutes, distance.Seconds);
        }
        else
        {
            CancelInvoke("Countdown");
            ShowSpinBtn(true);
        }
        SecondsRemaining--;
    }

    public void OnClickSpinNow()
    {
        OnSpin(() =>
        {
            Reset();
        });
    }

    public void OnSpin(Action action = null)
    {
        SoundController.Instance.PlayOnce(SoundType.LuckySpinRotate);
        Overlay.SetActive(true);
        SpinSkeleton.Play("attack", true);

        Spin.transform.localEulerAngles = Vector3.zero;

        var random = UnityEngine.Random.Range(0, 100);
        var index = LuckySpinItems.FindIndex(item => item.ProbabilityRange.x <= random && random <= item.ProbabilityRange.y);
        var endValue = Vector3.forward * (-360 * 5 + index * 45 + UnityEngine.Random.Range(5, 40f));

        Spin.DOLocalRotate(endValue, 3, RotateMode.FastBeyond360).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Overlay.SetActive(false);
            SpinSkeleton.Play("done", true);

            GenerateBonus(LuckySpinItems[index], () =>
            {
                LuckySpinDatas.LuckySpinTimeStart = DateTime.Now.ToString();
                SpinSkeleton.Play("idle", true);
                action?.Invoke();
            });
        });
    }

    public void GenerateBonus(LuckySpinItem item, Action action)
    {
        if (item.LuckySpinType == LuckySpinType.Coin)
        {
            GenerateCoin(item, action);
        }
        else
        {
            GenerateCandy(item, action);
        }
    }

    public void GenerateCoin(LuckySpinItem item, Action action)
    {
        int coinTotal = Data.CoinTotal + item.Value;
        CoinGeneration.GenerateCoin(() =>
        {
            Data.CoinTotal++;
        }, () =>
        {
            Data.CoinTotal = coinTotal;
            action?.Invoke();
        },
        item.Image.gameObject);
    }

    public void GenerateCandy(LuckySpinItem item, Action action)
    {
        int candyTotal = Data.TotalGoldMedal + item.Value;
        CandyGeneration.SetNumberCoin(item.Value);
        CandyGeneration.GenerateCoin(() =>
        {
            Data.TotalGoldMedal++;
        }, () =>
        {
            Data.TotalGoldMedal = candyTotal;
            action?.Invoke();

        },
        item.Image.gameObject);
    }

    public void OnClickFree()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            OnSpin();
        });
    }

    protected override void BeforeDismiss()
    {
        base.BeforeDismiss();

        CancelInvoke("Countdown");
    }
}
