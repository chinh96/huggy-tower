using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class LibraryItemInfoPopup : Popup
{
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI levelUnlock;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        if (Data.LibraryItemInfo.Image == null)
        {
            image.gameObject.SetActive(false);
            skeletonGraphic.gameObject.SetActive(true);
            skeletonGraphic.transform.localPosition = Data.LibraryItemInfo.Offset2;
            skeletonGraphic.transform.localScale = Data.LibraryItemInfo.Scale2;
            skeletonGraphic.skeletonDataAsset = Data.LibraryItemInfo.SkeletonDataAsset;
            skeletonGraphic.initialFlipX = Data.LibraryItemInfo.IsFlipX2;
            skeletonGraphic.initialSkinName = Data.LibraryItemInfo.LibraryAnimation.SkinName;
            skeletonGraphic.Initialize(true);
            skeletonGraphic.Play(Data.LibraryItemInfo.LibraryAnimation.Idle, true);
        }
        else
        {
            skeletonGraphic.gameObject.SetActive(false);
            image.gameObject.SetActive(true);
            image.transform.localScale = Vector3.one * .8f;
            image.sprite = Data.LibraryItemInfo.Image;
            image.SetNativeSize();
            image.transform.localScale = Data.LibraryItemInfo.Scale2;
        }

        name.text = Data.LibraryItemInfo.Name;
        //levelUnlock.text = $"Unlock Lv {Data.LibraryItemInfo.LevelUnlock}";
        levelUnlock.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", " LV " + Data.LibraryItemInfo.LevelUnlock, true);
    }
}
