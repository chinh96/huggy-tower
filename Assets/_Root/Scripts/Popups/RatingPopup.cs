public class RatingPopup : Popup
{
    public void OnClickOKButton()
    {
        RatingController.Instance.LinkToStore();
        Close();
    }
}
