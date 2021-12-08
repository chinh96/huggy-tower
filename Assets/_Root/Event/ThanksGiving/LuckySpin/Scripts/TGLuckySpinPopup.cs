using DG.Tweening;
using System;
using UnityEngine;
using TMPro;
using Spine.Unity;
using System.Collections.Generic;

public class TGLuckySpinPopup : Popup
{
    public GameObject SpinNowBtn;
    public GameObject SpinFreeBtn;
    public GameObject CountdownBtn;
    public TextMeshProUGUI CountdownText;
    public SkeletonGraphic SpinSkeleton;
    public Transform Spin;
    public int SpinNumber = 10;
    public List<TGLuckySpinItem> LuckySpinItems;
    public float radian;
    public CoinGeneration CoinGeneration;
    public CoinGeneration TurkeyGeneration;
    public GameObject Overlay;
    public TextMeshProUGUI CountText;

    private int count;

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

    protected override void BeforeShow()
    {
        base.BeforeShow();

        if (TGDatas.LuckySpinTimeStart == "")
        {
            TGDatas.LuckySpinTimeStart = DateTime.Now.AddMinutes(-20).ToString();
        }

        InvokeRepeating("Countdown", 0, 1);
        SpinSkeleton.Play("idle", true);
    }

    private void ShowSpinBtn(bool state)
    {
        SpinNowBtn.SetActive(state);
        CountdownBtn.SetActive(!state);
        SpinFreeBtn.SetActive(!state);
    }

    public void Countdown()
    {
        var timeRemaining = DateTime.Now - DateTime.Parse(TGDatas.LuckySpinTimeStart);
        var totalMinutes = (int)(timeRemaining.TotalMinutes / 10);
        count = totalMinutes > 2 ? 1 : totalMinutes;

        if (count < 0)
        {
            count = 0;
        }
        if (count > 1)
        {
            count = 1;
        }
        CountText.text = $"{count}/1";
        if (count == 0)
        {
            ShowSpinBtn(false);
            var countdown = DateTime.Parse(TGDatas.LuckySpinTimeStart).AddMinutes(10) - DateTime.Now;
            var distance = TimeSpan.FromSeconds(countdown.TotalSeconds);
            CountdownText.text = string.Format("{0:00}:{1:00}", distance.Minutes, distance.Seconds);
        }
        else
        {
            ShowSpinBtn(true);
        }
    }

    public void OnClickSpinNow()
    {
        OnSpin(() =>
        {
            if (count == 1)
            {
                TGDatas.LuckySpinTimeStart = DateTime.Now.ToString();
            }
            else
            if (count == 2)
            {
                TGDatas.LuckySpinTimeStart = DateTime.Now.AddMinutes(-10).ToString();
            }
            CountText.text = $"{count - 1}/1";

            EventController.LuckySpinChanged?.Invoke();
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
        if (LuckySpinItems[index].LuckySpinType == TGLuckySpinType.Skin && ResourcesController.Hero.SkinLuckySpin.IsUnlocked)
        {
            index = 0;
        }
        var endValue = Vector3.forward * (-360 * 5 + index * 45 + UnityEngine.Random.Range(10, 35f));

        Spin.DOLocalRotate(endValue, 3, RotateMode.FastBeyond360).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Overlay.SetActive(false);
            SpinSkeleton.Play("done", true);

            GenerateBonus(LuckySpinItems[index], () =>
            {
                SpinSkeleton.Play("idle", true);
                action?.Invoke();
            });
        });
    }

    public void GenerateBonus(TGLuckySpinItem item, Action action)
    {
        if (item.LuckySpinType == TGLuckySpinType.Coin)
        {
            GenerateCoin(item, action);
        }
        else if (item.LuckySpinType == TGLuckySpinType.Turkey)
        {
            GenerateTurkey(item, action);
        }
        else
        {
            PopupController.Instance.Show<TGLuckySpinGiftPopup>();
            action?.Invoke();
        }
    }

    public void GenerateCoin(TGLuckySpinItem item, Action action)
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

    public void GenerateTurkey(TGLuckySpinItem item, Action action)
    {
        int turkeyTotal = TGDatas.TotalTurkeyText + item.Value;
        TurkeyGeneration.SetNumberCoin(item.Value);
        TurkeyGeneration.GenerateCoin(() =>
        {
            TGDatas.TotalTurkeyText++;
        }, () =>
        {
            TGDatas.TotalTurkeyText = turkeyTotal;
            TGDatas.TotalTurkey = turkeyTotal;

            action?.Invoke();
        },
        item.Image.gameObject);
    }

    public void OnClickFree()
    {
        AnalyticController.LogEvent("FreeSpinClick", new Firebase.Analytics.Parameter[] { });
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