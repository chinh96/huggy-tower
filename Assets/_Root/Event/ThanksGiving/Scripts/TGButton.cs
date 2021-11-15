using UnityEngine;

public class TGButton : MonoBehaviour
{
    private bool isShow;

    private void Awake()
    {
        isShow = TGDatas.IsInTG;
        gameObject.SetActive(isShow);

        if (TGDatas.IsAfterTG)
        {
            SkinData data = ResourcesController.SkinsTG.Find(data => data.TGType == TGType.Top100);
            if (!data.IsUnlocked)
            {
                TGRankController.Instance.IsTop100(() =>
                {
                    data.IsUnlocked = true;
                    Data.CurrentSkinPrincess = data.SkinName;
                    PopupController.Instance.Show<TGReceiveSkinPopup>();
                });
            }
        }
    }

    private void Start()
    {
        HomeController.Instance.BackgroundTG.SetActive(isShow);
    }

    public void OnClick()
    {
        PopupController.Instance.Show<TGPopup>();
    }
}
