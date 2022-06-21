using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CurrentSkin : MonoBehaviour
{
    [SerializeField] private EUnitType eUnitType;
    [SerializeField] private SkeletonGraphic character;

    private SubjectSkinChange m_subjectSkinChange;

    private bool isHome = false;
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
    private void Update(){
        if(isHome == false && GetComponentInParent<HomeController>() != null){
            isHome = true;
            character.AnimationState.SetAnimation(0, "Home", true).MixDuration = 0;
        }
        else if(isHome == true && GetComponentInParent<HomeController>() == null){
            isHome = false;
            character.AnimationState.SetAnimation(0, "Idle", true).MixDuration = 0;
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
                isHome = false;
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
