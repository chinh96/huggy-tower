using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening;

public class FacebookPopup : Popup
{
    [SerializeField] private GameObject background1;
    [SerializeField] private GameObject background2;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        Reset();
    }

    private void Reset()
    {
        HideAll();
        CheckBackground();
    }

    private void CheckBackground()
    {
        if (Data.IsJoinedFb)
        {
            background2.SetActive(true);
        }
        else
        {
            background1.SetActive(true);
        }
    }

    private void HideAll()
    {
        background1.SetActive(false);
        background2.SetActive(false);
    }

    public void OnClickFacebookButton()
    {
        DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
        {
            Data.CoinTotal += 500;
            Data.IsJoinedFb = true;
            Reset();
        });

        Application.OpenURL("https://www.facebook.com/groups/hero.tower.wars");
    }
}
