using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPopup : Popup
{
    [SerializeField] private List<World> worlds;

    private World worldCurrent;

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        EventController.CastleBuilded = Build;
        EventController.CastleReseted = Reset;
        Reset();
    }

    private void Reset()
    {
        worlds.ForEach(item =>
        {
            if (item.WorldType == Data.WorldCurrent)
            {
                worldCurrent = item;
                item.gameObject.SetActive(true);
                item.Reset();
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        });
    }

    public void OnClickBuildButton()
    {
        PopupController.Instance.Show<CastlePopup>(null, ShowAction.DoNothing);
    }

    public void OnClickWorldButton()
    {
        PopupController.Instance.Show<WorldCollectPopup>(null, ShowAction.DoNothing);
    }

    public void Build(int castleIndex)
    {
        worldCurrent.Build(castleIndex);
    }
}