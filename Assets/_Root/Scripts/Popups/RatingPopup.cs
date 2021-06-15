public class RatingPopup : Popup
{
    public void LinkToStore()
    {
        RatingController.Instance.LinkToStore();
        Close();
    }
}
