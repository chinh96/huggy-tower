using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlePopup : Popup
{
    [SerializeField] private List<Castle> castles;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        for (int i = 0; i < castles.Count; i++)
        {
            Castle castle = castles[i];
            castle.Init(i, this);
        }
    }
}
