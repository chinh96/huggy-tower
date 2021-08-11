using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using I2.Loc;

public class LibraryPopup : Popup
{
    [SerializeField] private Transform content;
    [SerializeField] private LibraryItem libraryItem;
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image image;
    [SerializeField] private GameObject LibraryItemComingSoon;

    private List<LibraryItem> libraryItems = new List<LibraryItem>();
    private LibraryData libraryDataCurrent;

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        content.Clear();
        ResourcesController.Library.LibraryDatas.ForEach(data =>
        {
            libraryItems.Add(Instantiate(libraryItem, content));
        });

        Instantiate(LibraryItemComingSoon, content);
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        int index = 0;
        libraryItems.ForEach(item =>
        {
            var data = ResourcesController.Library.LibraryDatas[index];
            item.Init(data, index, this);
            index++;
        });

        Reset(0);
    }

    public void Reset(int indexActive)
    {
        int index = 0;
        libraryItems.ForEach(item =>
        {
            if (index == indexActive)
            {
                item.SetActive();
            }
            else
            {
                item.Reset();
            }
            index++;
        });

        libraryDataCurrent = ResourcesController.Library.LibraryDatas[indexActive];

        if (libraryDataCurrent.Image == null)
        {
            image.gameObject.SetActive(false);
            skeletonGraphic.gameObject.SetActive(true);
            skeletonGraphic.transform.localPosition = libraryDataCurrent.Offset;
            skeletonGraphic.transform.localScale = libraryDataCurrent.Scale;
            skeletonGraphic.transform.localRotation = Quaternion.Euler(libraryDataCurrent.Rotation.x, libraryDataCurrent.Rotation.y, libraryDataCurrent.Rotation.z);
            skeletonGraphic.skeletonDataAsset = libraryDataCurrent.SkeletonDataAsset;
            skeletonGraphic.initialFlipX = libraryDataCurrent.IsFlipX;
            skeletonGraphic.initialSkinName = libraryDataCurrent.LibraryAnimation.SkinName;
            skeletonGraphic.Initialize(true);
            skeletonGraphic.Play(libraryDataCurrent.LibraryAnimation.Idle, true);
        }
        else
        {
            skeletonGraphic.gameObject.SetActive(false);
            image.gameObject.SetActive(true);
            image.sprite = libraryDataCurrent.Image;
            image.SetNativeSize();
            image.transform.localScale = libraryDataCurrent.Scale;
        }

        name.text = libraryDataCurrent.Name;
        //description.text = libraryDataCurrent.Description;
        description.GetComponent<Localize>().SetTerm("LibraryPopup_txt"+ libraryDataCurrent.Name + "Description");
    }
}
