using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;
public class ItemEventNoel : MonoBehaviour
{
    [SerializeField] private GameObject claimActiveButton;
    [SerializeField] private GameObject claimDisableButton;
    [SerializeField] private GameObject keyLock;

    [SerializeField] private GameObject doneIcon;

    [SerializeField] private SkeletonGraphic hero;
    [SerializeField] private TextMeshProUGUI textSock;
    ItemConfigCollectEvent cfg;
    private onClickHandle clickCallBack;
    StateClaimDailyEvent _stateClaim;

    public delegate void onClickHandle(ItemConfigCollectEvent cfg, GameObject my);
    public void InitItemEventNoel(StateClaimDailyEvent state, ItemConfigCollectEvent configEvent, onClickHandle clickcb)
    {
        this.cfg = configEvent;
        this.clickCallBack = clickcb;
        SetStateItem(state);
        setSock(configEvent.NumCandyXmas);

        if (configEvent.NumCandyXmas == -1)
        {
            claimActiveButton.SetActive(false);
            claimDisableButton.SetActive(true);
            if (TGDatas.IsAfter5Days)
            {
                TGRankController.Instance.IsTop100(() =>
                {
                    claimActiveButton.SetActive(true);
                    claimDisableButton.SetActive(false);
                    keyLock.SetActive(false);
                });
            }

        }
    }
    void setSock(int sockXmas)
    {
        if (sockXmas > 0)
        {
            textSock.gameObject.SetActive(true);
            setTextSock("" + sockXmas);
        }
        else
        {
            textSock.gameObject.SetActive(false);
        }
    }
    void setTextSock(string text)
    {
        textSock.text = text;
    }

    public void OnClickClaim()
    {
        clickCallBack(cfg, gameObject);
        SetStateItem(StateClaimDailyEvent.CLAIMED);
    }







    void SetStateItem(StateClaimDailyEvent stateClaim)
    {
        this._stateClaim = stateClaim;
        doneIcon.SetActive(false);
        switch (stateClaim)
        {
            case StateClaimDailyEvent.CLAIMED:
                doneIcon.SetActive(true);
                // background.sprite = spriteCoinClaimed;
                claimActiveButton.SetActive(false);

                claimDisableButton.SetActive(false);
                if (keyLock)
                {
                    keyLock.SetActive(false);
                }
                break;
            case StateClaimDailyEvent.WAITING_CLAIM:
                // doneIcon.SetActive(false);
                // background.sprite = spriteCoinNotClaimed;
                claimActiveButton.SetActive(false);

                claimDisableButton.SetActive(true);
                break;
            case StateClaimDailyEvent.CAN_CLAIM:
                // doneIcon.SetActive(false);
                // background.sprite = spriteCoinCurrent;
                if (keyLock)
                {
                    keyLock.SetActive(false);
                }
                claimActiveButton.SetActive(true);

                claimDisableButton.SetActive(false);
                break;

        }

    }

}
