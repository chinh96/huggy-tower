using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Spine.Unity;
using UnityEngine.UI;
using I2.Loc;

public enum StateClaimDailyEvent
{
    CLAIMED = 1,
    WAITING_CLAIM = 2,
    CAN_CLAIM = 3

}
public class DailyRewardEventItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI textHero;
    [SerializeField] private TextMeshProUGUI textCandy;

    [SerializeField] private Image coinIcon;
    [SerializeField] private Image candyIcon;
    [SerializeField] private GameObject doneIcon;
    [SerializeField] private SkeletonGraphic hero;
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject claimPendingButton;
    [SerializeField] private GameObject claimDisableButton;
    [SerializeField] private Image background;

    [SerializeField] private Sprite spriteCoinClaimed;
    [SerializeField] private Sprite spriteCoinCurrent;
    [SerializeField] private Sprite spriteCoinNotClaimed;

    [SerializeField] private GameObject bgShadownCover;
    public delegate void onClickHandle(int day, ItemConfigEvent cfg, GameObject my);
    private onClickHandle clickCallBack;

    private ItemConfigEvent cfg;
    int _day = 0;
    bool haveCoin = false;
    bool haveCandy = false;


    StateClaimDailyEvent _stateClaim;


    public void InitDailyItem(int day, StateClaimDailyEvent stateClaim, ItemConfigEvent configEvent, onClickHandle clickcb)
    {
        this.clickCallBack = clickcb;
        this.cfg = configEvent;
        SetDay(day);
        SetCoin(configEvent.Coin);
        SetStateItem(stateClaim);
        SetSkin(configEvent.SkinId);
        setTextCandy(configEvent.CandyXmas);
        updateUi();
    }
    void SetTextDay(string text)
    {

        dayText.text = text;
        // dayText.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", text, true);
    }
    void SetDay(int day)
    {
        this._day = day;
        SetTextDay("Day " + (day + 1).ToString());
    }

    void setTextCandy(int candyXmas)
    {
        if (candyXmas > 0)
        {
            // candyIcon.gameObject.SetActive(true);
            textCandy.gameObject.SetActive(true);
            setTextCandy("+" + candyXmas);
            haveCandy = true;
        }
        else
        {

            candyIcon.gameObject.SetActive(false);
            textCandy.gameObject.SetActive(false);
        }
    }

    void SetTextCoin(string text)
    {
        coinText.text = text;
    }

    void setTextCandy(string text)
    {
        textCandy.text = text;
    }

    void SetCoin(int coin)
    {
        if (coin > 0)
        {
            // coinIcon.gameObject.SetActive(true);
            textHero.gameObject.SetActive(false);
            coinText.gameObject.SetActive(true);
            SetTextCoin($"{coin}");
            haveCoin = true;
        }
        else
        {
            coinIcon.gameObject.SetActive(false);
            coinText.gameObject.SetActive(false);
        }

    }
    void SetSkin(int idSkin)
    {
        if (idSkin == 0 || idSkin == -1)
        {
            textHero.gameObject.SetActive(false);
            hero.gameObject.SetActive(false);
            return;
        }
        textHero.gameObject.SetActive(true);
        var data = ResourcesController.Hero.SkinDatas[idSkin];
        SetSkeletonGraphicSkinHero(data.SkinName);
    }
    void SetSkeletonGraphicSkinHero(string skinName)
    {
        hero.ChangeSkin(skinName);
    }
    void SetSkeletonGraphicSkinPrincess()
    {

    }
    void SetStateItem(StateClaimDailyEvent stateClaim)
    {
        this._stateClaim = stateClaim;
        doneIcon.SetActive(false);
        bgShadownCover.SetActive(false);
        switch (stateClaim)
        {
            case StateClaimDailyEvent.CLAIMED:
                doneIcon.SetActive(true);
                bgShadownCover.SetActive(true);
                // background.sprite = spriteCoinClaimed;
                claimButton.SetActive(false);
                claimPendingButton.SetActive(false);
                claimDisableButton.SetActive(true);
                break;
            case StateClaimDailyEvent.WAITING_CLAIM:
                // doneIcon.SetActive(false);
                // background.sprite = spriteCoinNotClaimed;
                claimButton.SetActive(false);
                // claimPendingButton.SetActive(true);
                claimDisableButton.SetActive(true);
                break;
            case StateClaimDailyEvent.CAN_CLAIM:
                // doneIcon.SetActive(false);
                // background.sprite = spriteCoinCurrent;
                claimButton.SetActive(true);
                claimPendingButton.SetActive(false);
                claimDisableButton.SetActive(false);
                break;

        }
    }
    public void OnClickClaim()
    {
        SetStateItem(StateClaimDailyEvent.CLAIMED);
        this.clickCallBack(_day, this.cfg, gameObject);
    }
    void updateUi()
    {
        if (haveCandy == true && haveCoin == true)
        {
            coinIcon.transform.localPosition = new Vector3(-52.4f, -31.7f, 0);
            candyIcon.transform.localPosition = new Vector3(49f, 0, 0);
        }
        else
        {
            if (haveCoin == true)
            {
                coinIcon.transform.localPosition = new Vector3(0, 10f, 0);
            }
            else
            {
                candyIcon.transform.localPosition = new Vector3(0, 0, 0);
            }


        }
    }
}
