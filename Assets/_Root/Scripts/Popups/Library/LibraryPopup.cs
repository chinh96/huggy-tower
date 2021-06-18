using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using DG.Tweening;

public class LibraryPopup : Popup
{
    [SerializeField] private Transform content;
    [SerializeField] private LibraryItem libraryItem;
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;

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

        skeletonGraphic.transform.localScale = libraryDataCurrent.Scale;
        skeletonGraphic.skeletonDataAsset = libraryDataCurrent.SkeletonDataAsset;
        skeletonGraphic.initialFlipX = libraryDataCurrent.IsFlipX;
        skeletonGraphic.initialSkinName = libraryDataCurrent.LibraryAnimation.SkinName;
        skeletonGraphic.Initialize(true);
        skeletonGraphic.Play(libraryDataCurrent.LibraryAnimation.Idle, true);

        name.text = libraryDataCurrent.Name;
        description.text = libraryDataCurrent.Description;
    }
}
