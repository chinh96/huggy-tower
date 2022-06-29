using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LosePopup : Popup
{
    [SerializeField] private ProgressGift progressGift;
    [SerializeField] private GameObject tapToReplayButton;
    [SerializeField] private GameObject huggy;
    [SerializeField] private LevelText levelText;
    protected override void BeforeShow()
    {
        base.BeforeShow();
        levelText.ChangeLevel();

        tapToReplayButton.SetActive(false);
        progressGift.Reset();
    }

    protected override void AfterShown()
    {
        base.AfterShown();
        huggy.GetComponent<HeroWinLoseController>().PlayLose();
        DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
        {
            tapToReplayButton.SetActive(true);
        });
    }

    public void OnClickReplay()
    {
        GameController.Instance.OnReplayLevel();
    }

    public void OnClickHomeButton()
    {
        GameController.Instance.OnBackToHome();
    }

    public void OnClickSkipButton()
    {
        GameController.Instance.OnSkipLevel(AnalyticController.AdjustLogEventSkipLoseLevel);
    }
}
