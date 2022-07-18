using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlePopup : Popup
{
    [SerializeField] private List<Castle> castles;

    private WorldResources worldCurrent;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        //this.worldCurrent = ResourcesController.Universe.WorldCurrent;

        for (int i = 0; i < castles.Count; i++)
        {
            Castle castle = castles[i];
            castle.Init(i, this, worldCurrent);
        }
        
        if (!Data.FlagFirstTimeVisitCastle)
        {
            Data.FlagFirstTimeVisitCastle = true;
            AnalyticController.AdjustLogEventBuildCastle();
        }
    }
}
