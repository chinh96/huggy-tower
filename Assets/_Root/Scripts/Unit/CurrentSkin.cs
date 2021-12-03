using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CurrentSkin : MonoBehaviour
{
    [SerializeField] private EUnitType eUnitType;
    [SerializeField] private SkeletonGraphic character;

    private SubjectSkinChange m_subjectSkinChange;

    private void Start()
    {
        m_subjectSkinChange = transform.GetComponent<SubjectSkinChange>();
        ChangeCurrentSkin();
        switch (eUnitType)
        {
            case EUnitType.Hero:
                EventController.CurrentSkinHeroChanged += ChangeCurrentSkin;
                break;
            case EUnitType.Princess:
                EventController.CurrentSkinPrincessChanged += ChangeCurrentSkin;
                break;
        }
    }

    private void ChangeCurrentSkin()
    {
        switch (eUnitType)
        {
            case EUnitType.Hero:
                character.ChangeSkin(Data.CurrentSkinHero, eUnitType);
                var SkinData = ResourcesController.Hero.GetSkinDataById(Data.currentSkinHeroId);
                if (SkinData != null) m_subjectSkinChange.Next(SkinData);
                break;
            case EUnitType.Princess:
                character.ChangeSkin(Data.CurrentSkinPrincess, eUnitType);
                break;
        }
    }

    public void ChangeCurrentSkin(string skinName)
    {
        character.ChangeSkin(skinName, eUnitType);
    }
}
