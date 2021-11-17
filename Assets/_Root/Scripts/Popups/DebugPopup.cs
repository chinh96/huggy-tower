using TMPro;

public class DebugPopup : Popup
{
    public TMP_InputField LevelInput;
    public TMP_InputField CoinInput;
    public TMP_InputField TurkeyInput;

    public void OnClickOK()
    {
        if (CoinInput.text != "")
        {
            Data.CoinTotal = int.Parse(CoinInput.text);
        }
        if (LevelInput.text != "")
        {
            Data.CurrentLevel = int.Parse(LevelInput.text);
        }
        if (TurkeyInput.text != "")
        {
            TGDatas.TotalTurkey = int.Parse(TurkeyInput.text);
            TGDatas.TotalTurkeyText = int.Parse(TurkeyInput.text);
        }
        Close();
    }
}
