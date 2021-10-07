using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescuePartyReceiveSkinPopup : Popup
{
    public void OnClickClaim()
    {
        SkinData data = ResourcesController.SkinRescuePartys.Find(data => data.RescuePartyType == RescuePartyType.Top100);
        data.IsUnlocked = true;
        Data.CurrentSkinPrincess = data.SkinName;
        Close();
    }
}
