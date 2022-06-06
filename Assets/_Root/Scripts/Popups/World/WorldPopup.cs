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

    public GameObject GetWorldCurrentGameObject(){
        return this.worldCurrent.gameObject;
    }

    public void Build(int castleIndex)
    {
        worldCurrent.Build(castleIndex);
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        this.worldCurrent.GetComponent<World>().ReturnOriginalPosition();
        SoundController.Instance.PlayBackground(SoundType.BackgroundCastle);
    }

    public void OnClickBuildButton()
    {
        PopupController.Instance.Show<CastlePopup>(null, ShowAction.DoNothing);
    }

    public void OnClickWorldPopup()
    {
        PopupController.Instance.Show<WorldCollectPopup>(null, ShowAction.DoNothing);
    }

    public override void Close()
    {
        base.Close();

        if (GameController.Instance != null)
        {
            SoundController.Instance.PlayBackground(SoundType.BackgroundInGame);
        }
        else
        {
            SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
        }

        HomeController.Instance.ResetBackground();
    }
}
