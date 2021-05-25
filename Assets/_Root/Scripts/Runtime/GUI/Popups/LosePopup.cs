using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LosePopup : Popup
{
    [SerializeField] private ProgressGift progressGift;
    [SerializeField] private GameObject tapToReplayButton;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        tapToReplayButton.SetActive(false);
        progressGift.Reset();
    }

    protected override void AfterShown()
    {
        base.AfterShown();

        DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
        {
            tapToReplayButton.SetActive(true);
        });
    }

    public void OnClickReplay()
    {
        GameController.Instance.OnReplayLevel();
        Close();
    }

    public void OnClickHomeButton()
    {
        GameController.Instance.OnBackToHome();
    }
}
