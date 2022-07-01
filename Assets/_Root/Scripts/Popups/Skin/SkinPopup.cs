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
    [SerializeField] private LevelText levelText;
    private SubjectSkinChange m_subjectSkinChange;

    private SkinResources skinResources;

    private List<SkinRow> skinRows = new List<SkinRow>();

    public EUnitType EUnitType { get => eUnitType; set => eUnitType = value; }

    private void Awake()
    {
        switch (eUnitType)
        {
            case EUnitType.Hero:
                skinResources = ResourcesController.Hero;
                EventController.SkinPopupReseted += Reset; // Because Princess Skinpopup doesn't have enough skin
                break;
            case EUnitType.Princess:
                skinResources = ResourcesController.Princess;
                break;
        }

        if (!m_subjectSkinChange)
        {
            m_subjectSkinChange = character.GetComponent<SubjectSkinChange>();
        }
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
        levelText.ChangeLevel();
        Reset();
    }

    public void Reset()
    {
        switch (eUnitType)
        {
            case EUnitType.Hero:
                character.ChangeSkin(Data.CurrentSkinHero, eUnitType);
                break;
            case EUnitType.Princess:
                character.ChangeSkin(Data.CurrentSkinPrincess, eUnitType);
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

    public void ChangeCharacterSkin(SkinData skinData)
    {
        m_subjectSkinChange.Next(skinData);
        character.ChangeSkin(skinData.SkinName, eUnitType);
    }

    public void ResetDock()
    {
        skinRows.ForEach(skinRow => skinRow.ResetDock());
    }

    public void ResetUsedLabel()
    {
        skinRows.ForEach(skinRow => skinRow.ResetUsedLabel());
    }

    public void ResetFx()
    {
        skinRows.ForEach(skinRow => skinRow.ResetFx());
    }

    public void ShowPrincessPopup()
    {
        PopupController.Instance.Show<SkinPrincessPopup>(null, ShowAction.DoNothing);
    }
}
