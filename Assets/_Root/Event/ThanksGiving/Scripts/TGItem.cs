using UnityEngine;

public class TGItem : MonoBehaviour
{
    public TGType Type;
    [SerializeField] private GameObject claimActiveButton;
    [SerializeField] private GameObject claimDisableButton;
    [SerializeField] private GameObject keyLock;

    [SerializeField] private GameObject iconDone;

    private SkinData data;
    private TGPopup tgRankPopup;

    public bool IsClaimedGoldEventThanksGiving
    {
        get => Data.GetBool("IsClaimedGoldEventThanksGiving", false);
        set => Data.SetBool("IsClaimedGoldEventThanksGiving", value);
    }

    public void Init(SkinData data, TGPopup tgRankPopup)
    {
        this.data = data;
        this.tgRankPopup = tgRankPopup;
    }

    public void Reset()
    {
        if (Type == TGType.Gold)
        {
            claimActiveButton.SetActive(TGDatas.TotalTurkey >= 50 && !IsClaimedGoldEventThanksGiving);
            claimDisableButton.SetActive(TGDatas.TotalTurkey < 50 && !IsClaimedGoldEventThanksGiving);
            iconDone.SetActive(IsClaimedGoldEventThanksGiving);
            keyLock.SetActive(TGDatas.TotalTurkey < 50 && !IsClaimedGoldEventThanksGiving);
        }
        else if (Type == TGType.Hero)
        {
            claimActiveButton.SetActive(TGDatas.TotalTurkey >= data.NumberTurkeyTarget && !data.IsUnlocked);
            claimDisableButton.SetActive(TGDatas.TotalTurkey < data.NumberTurkeyTarget && !data.IsUnlocked);
            iconDone.SetActive(data.IsUnlocked);
            keyLock.SetActive(TGDatas.TotalTurkey < data.NumberTurkeyTarget && !data.IsUnlocked);
        }
        else if (Type == TGType.Top100)
        {
            claimActiveButton.SetActive(false);
            claimDisableButton.SetActive(true);
            iconDone.SetActive(false);
            keyLock.SetActive(true);
            if (data.IsUnlocked)
            {
                claimActiveButton.SetActive(false);
                claimDisableButton.SetActive(false);
                iconDone.SetActive(true);
                keyLock.SetActive(false);
            }
            else if (TGDatas.IsAfter5Days)
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

    public void OnClickClaim()
    {
        if (Type == TGType.Gold)
        {
            Data.CoinTotal += 10000;
            IsClaimedGoldEventThanksGiving = true;
        }
        else if (Type == TGType.Hero)
        {
            Data.CurrentSkinHero = data.SkinName;
            data.IsUnlocked = true;
        }
        else if (Type == TGType.Top100)
        {
            Data.CurrentSkinPrincess = data.SkinName;
            Data.CoinTotal += 20000;
            data.IsUnlocked = true;
        }

        tgRankPopup.Reset();
    }
}
