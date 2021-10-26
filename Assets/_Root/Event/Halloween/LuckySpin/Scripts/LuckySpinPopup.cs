using DG.Tweening;
using System;
using UnityEngine;
using TMPro;
using Spine.Unity;

public class LuckySpinPopup : Popup
{
    public GameObject SpinNowBtn;
    public GameObject CountdownBtn;
    public TextMeshProUGUI CountdownText;
    public SkeletonGraphic SpinSkeleton;
    public Transform Spin;

    private int SecondsRemaining;

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
        SecondsRemaining = (int)(DateTime.Parse(LuckySpinDatas.LuckySpinTimeStart).AddSeconds(5) - DateTime.Now).TotalSeconds;
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
        SpinSkeleton.Play("attack", true);
        Spin.DORotate(Spin.eulerAngles + Vector3.forward * 180, .1f).SetLoops(10).SetEase(Ease.Linear).OnComplete(() =>
        {
            SpinSkeleton.Play("done", true);
            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
            {
                LuckySpinDatas.LuckySpinTimeStart = DateTime.Now.ToString();
                Reset();
            });
        });
    }

    public void OnClickFree()
    {

    }

    protected override void BeforeDismiss()
    {
        base.BeforeDismiss();

        CancelInvoke("Countdown");
    }
}
