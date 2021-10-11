using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugPopup : Popup
{
    public TMP_InputField LevelInput;
    public TMP_InputField CoinInput;

    public void OnClickOK()
    {
        Data.CoinTotal = int.Parse(CoinInput.text);
        Data.CurrentLevel = int.Parse(LevelInput.text);
        Close();
    }
}
