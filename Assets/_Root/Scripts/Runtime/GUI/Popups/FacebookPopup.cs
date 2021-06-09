using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FacebookPopup : Popup
{
    [SerializeField] private List<GameObject> backgrounds;

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

    private void HideAll()
    {
        backgrounds.ForEach(item => item.SetActive(false));
    }

    private void CheckBackground()
    {
        backgrounds[Data.JoinFbProgress].SetActive(true);
    }

    public void OnClickFacebookButton()
    {
        Data.JoinFbProgress++;

        DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
        {
            Reset();
        });

        Application.OpenURL("https://www.facebook.com/groups/hero.tower.wars");
    }

    public void OnClickClaimButton()
    {
        Data.JoinFbProgress++;
        Data.CoinTotal += 500;
        Close();
    }
}
