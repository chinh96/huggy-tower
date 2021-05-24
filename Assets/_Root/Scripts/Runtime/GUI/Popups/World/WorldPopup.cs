using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPopup : Popup
{
    [SerializeField] private List<World> worlds;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        worlds.ForEach(item => item.gameObject.SetActive(item.WorldType == Data.WorldCurrent));
    }

    public void OnClickHomeButton()
    {
        GameController.Instance.OnBackToHome();
    }

    public void OnClickBuildButton()
    {

    }

    public void OnClickWorldButton()
    {

    }
}
