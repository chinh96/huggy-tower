using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LibraryItem : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private GameObject active;
    [SerializeField] private GameObject locked;
    [SerializeField] private TextMeshProUGUI text;

    private LibraryData libraryData;
    private LibraryPopup libraryPopup;
    private int index;

    public void Init(LibraryData libraryData, int index, LibraryPopup libraryPopup)
    {
        this.libraryData = libraryData;
        this.libraryPopup = libraryPopup;
        this.index = index;

        text.text = "Lv:" + libraryData.LevelUnlock;
        avatar.sprite = libraryData.Sprite;
    }

    public void Reset()
    {
        if (libraryData.IsUnlocked)
        {
            SetNormal();
        }
        else
        {
            SetLocked();
        }
    }

    public void SetNormal()
    {
        active.SetActive(false);
        locked.SetActive(false);
    }

    public void SetActive()
    {
        active.SetActive(true);
        locked.SetActive(false);
    }

    public void SetLocked()
    {
        active.SetActive(false);
        locked.SetActive(true);
    }

    public void OnClick()
    {
        if (libraryData.IsUnlocked)
        {
            libraryPopup.Reset(index);
        }
    }
}
