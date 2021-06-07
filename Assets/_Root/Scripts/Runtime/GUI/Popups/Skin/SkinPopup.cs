using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SkinPopup : Popup
{
    [SerializeField] private EUnitType eUnitType;
    [SerializeField] private Transform content;
    [SerializeField] private SkinRow skinRow;
    [SerializeField] private SkeletonGraphic character;

    private SkinResources skinResources;

    private List<SkinRow> skinRows = new List<SkinRow>();

    public EUnitType EUnitType { get => eUnitType; set => eUnitType = value; }

    private void Awake()
    {
        switch (eUnitType)
        {
            case EUnitType.Hero:
                skinResources = ResourcesController.Hero;
                break;
            case EUnitType.Princess:
                skinResources = ResourcesController.Princess;
                break;
        }

        EventController.SkinPopupReseted = Reset;
    }

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        content.Clear();

        int index = 0;
        while (index < skinResources.SkinDatas.Count)
        {
            SkinRow row = Instantiate(skinRow, content);
            row.Init(skinResources, ref index, this);
            skinRows.Add(row);
        }
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        Reset();
    }

    public void Reset()
    {
        switch (eUnitType)
        {
            case EUnitType.Hero:
                character.ChangeSkin(Data.CurrentSkinHero);
                break;
            case EUnitType.Princess:
                character.ChangeSkin(Data.CurrentSkinPrincess);
                break;
        }

        int index = 0;
        skinRows.ForEach(item =>
        {
            item.Reset(skinResources.SkinDatas[index]);
            index++;
        });
    }

    public void OnClickAdsButton()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            Data.CoinTotal += 500;
        });
    }

    public void ChangeCharacterSkin(string skinName)
    {
        character.ChangeSkin(skinName);
    }
}
