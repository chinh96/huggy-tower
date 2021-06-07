using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCollectPopup : Popup
{
    [SerializeField] private Transform content;
    [SerializeField] private WorldCollect worldCollect;

    private List<WorldCollect> worldCollects = new List<WorldCollect>();

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        ResourcesController.Universe.Worlds.ForEach(item =>
        {
            WorldCollect world = Instantiate(worldCollect, content);
            world.Init(item, this);
            worldCollects.Add(world);
        });
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        worldCollects.ForEach(item => item.Reset());
    }
}
