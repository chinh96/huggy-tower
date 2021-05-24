using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPopup : Popup
{
    [SerializeField] private List<World> worlds;

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
