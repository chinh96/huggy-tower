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
    [SerializeField] private Image coinIcon;
    [SerializeField] private GameObject doneIcon;
    [SerializeField] private SkeletonGraphic hero;
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject claimPendingButton;
    [SerializeField] private GameObject claimDisableButton;
    [SerializeField] private Image background;

    [SerializeField] private Sprite spriteCoinClaimed;
    [SerializeField] private Sprite spriteCoinCurrent;
    [SerializeField] private Sprite spriteCoinNotClaimed;
    public delegate void onClickHandle(int day, ItemConfigEvent cfg, GameObject my);
    private onClickHandle clickCallBack;
    private ItemConfigEvent cfg;
    int _day = 0;
    StateClaimDailyEvent _stateClaim;


    public void InitDailyItem(int day, StateClaimDailyEvent stateClaim, ItemConfigEvent configEvent, onClickHandle clickcb)
    {
        this.clickCallBack = clickcb;
        this.cfg = configEvent;
        SetDay(day);
        SetCoin(configEvent.Coin);
        SetStateItem(stateClaim);
        SetSkin(configEvent.SkinId);
    }
    void SetTextDay(string text)
    {
        Debug.Log(text + "textDay");
        dayText.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", text, true);
    }
    void SetDay(int day)
    {
        this._day = day;
        SetTextDay((day + 1).ToString());
    }

    void SetTextCoin(string text)
    {
        coinText.text = text;
    }

    void SetCoin(int coin)
    {
        if (coin > 0)
        {
            textHero.gameObject.SetActive(false);
            coinText.gameObject.SetActive(true);
            SetTextCoin($"{coin}");
        }
        else
        {
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
        switch (stateClaim)
        {
            case StateClaimDailyEvent.CLAIMED:
                doneIcon.SetActive(true);
                background.sprite = spriteCoinClaimed;
                claimButton.SetActive(false);
                claimPendingButton.SetActive(false);
                claimDisableButton.SetActive(true);
                break;
            case StateClaimDailyEvent.WAITING_CLAIM:
                // doneIcon.SetActive(false);
                background.sprite = spriteCoinNotClaimed;
                claimButton.SetActive(false);
                claimPendingButton.SetActive(true);
                claimDisableButton.SetActive(false);
                break;
            case StateClaimDailyEvent.CAN_CLAIM:
                // doneIcon.SetActive(false);
                background.sprite = spriteCoinCurrent;
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
}
