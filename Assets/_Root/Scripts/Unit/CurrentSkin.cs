using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CurrentSkin : MonoBehaviour
{
    [SerializeField] private EUnitType eUnitType;
    [SerializeField] private SkeletonGraphic character;

    private void Start()
    {
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
