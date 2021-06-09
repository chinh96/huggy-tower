using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorldCollect : MonoBehaviour
{
    [SerializeField] private GameObject visitButton;
    [SerializeField] private GameObject lockButton;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Image background;
    [SerializeField] private GameObject noti;

    private WorldResources worldResources;
    private WorldCollectPopup worldCollectPopup;

    public void Init(WorldResources worldResources, WorldCollectPopup worldCollectPopup)
    {
        this.worldResources = worldResources;
        this.worldCollectPopup = worldCollectPopup;
    }

    public void Reset()
    {
        HideAll();
        background.sprite = worldResources.background;
        if (Data.CurrentLevel >= worldResources.LevelUnlock)
        {
            visitButton.SetActive(true);
            noti.SetActive(ResourcesController.Universe.IsNoti(worldResources));
        }
        else
        {
            lockButton.SetActive(true);
            level.text = $"Level {worldResources.LevelUnlock}";
        }
    }

    private void HideAll()
    {
        visitButton.SetActive(false);
        lockButton.SetActive(false);
    }

    public void OnClickVisitButton()
    {
        Data.WorldCurrent = worldResources.WorldType;
        worldCollectPopup.Close();
        EventController.CastleReseted();
    }
}
