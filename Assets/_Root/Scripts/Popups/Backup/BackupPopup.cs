using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackupPopup : Popup
{
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private Button btnOk;
    [SerializeField] private Button btnClose;
    [SerializeField] private Sprite greenArrow;
    [SerializeField] private Sprite redArrow;

    [Header("server")] [SerializeField] private TextMeshProUGUI txtLevelLeft;
    [SerializeField] private TextMeshProUGUI txtCoinLeft;
    [SerializeField] private TextMeshProUGUI txtTotalSkinLeft;
    [SerializeField] private TextMeshProUGUI txtTitleLeft;

    [Header("device")] [SerializeField] private TextMeshProUGUI txtLevelRight;
    [SerializeField] private Image arrowLevelRigth;
    [SerializeField] private TextMeshProUGUI txtCoinRight;
    [SerializeField] private Image arrowCoinRigth;
    [SerializeField] private TextMeshProUGUI txtTotalSkinRight;
    [SerializeField] private Image arrowSkinRigth;
    [SerializeField] private TextMeshProUGUI txtTitleRight;

    private Action _actionOk;
    private Action _actionClose;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionOk"></param>
    /// <param name="actionClose"></param>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="serverLevel"></param>
    /// <param name="serverCoin"></param>
    /// <param name="serverTotalSkin"></param>
    /// <param name="titleRight"></param>
    /// <param name="titleLeft"></param>
    /// <param name="isBackup"></param>
    public void Initialized(
        Action actionOk,
        Action actionClose,
        string message,
        string title,
        int serverLevel,
        int serverCoin,
        int serverTotalSkin,
        string titleRight,
        string titleLeft,
        bool isBackup)
    {
        _actionOk = actionOk;
        _actionClose = actionClose;
        txtMessage.text = message;
        txtTitle.text = title;
        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);

        int totalSkinUnlocked = ResourcesController.TotalSkinUnlocked();
        if (isBackup)
        {
            txtLevelRight.text = $"Level {serverLevel + 1}";
            txtCoinRight.text = $"{serverCoin}";
            txtTotalSkinRight.text = $"{serverTotalSkin}";

            txtLevelLeft.text = $"Level {Data.CurrentLevel + 1}";
            txtCoinLeft.text = $"{Data.CoinTotal}";
            txtTotalSkinLeft.text = $"{totalSkinUnlocked}";

            if (Data.CurrentLevel > serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = greenArrow;
            }
            else if (Data.CurrentLevel < serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = redArrow;
            }
            else
            {
                arrowLevelRigth.gameObject.SetActive(false);
            }

            if (Data.CoinTotal > serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = greenArrow;
            }
            else if (Data.CoinTotal < serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = redArrow;
            }
            else
            {
                arrowCoinRigth.gameObject.SetActive(false);
            }

            if (totalSkinUnlocked > serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = greenArrow;
            }
            else if (totalSkinUnlocked < serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = redArrow;
            }
            else
            {
                arrowSkinRigth.gameObject.SetActive(false);
            }
        }
        else
        {
            arrowCoinRigth.gameObject.SetActive(false);
            arrowLevelRigth.gameObject.SetActive(false);
            arrowSkinRigth.gameObject.SetActive(false);

            txtLevelLeft.text = $"Level {serverLevel + 1}";
            txtCoinLeft.text = $"{serverCoin}";
            txtTotalSkinLeft.text = $"{serverTotalSkin}";

            txtLevelRight.text = $"Level {Data.CurrentLevel + 1}";
            txtCoinRight.text = $"{Data.CoinTotal}";
            txtTotalSkinRight.text = $"{totalSkinUnlocked}";

            if (Data.CurrentLevel > serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = redArrow;
            }
            else if (Data.CurrentLevel < serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = greenArrow;
            }
            else
            {
                arrowLevelRigth.gameObject.SetActive(false);
            }

            if (Data.CoinTotal > serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = redArrow;
            }
            else if (Data.CoinTotal < serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = greenArrow;
            }
            else
            {
                arrowCoinRigth.gameObject.SetActive(false);
            }

            if (totalSkinUnlocked > serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = redArrow;
            }
            else if (totalSkinUnlocked < serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = greenArrow;
            }
            else
            {
                arrowSkinRigth.gameObject.SetActive(false);
            }
        }

        txtTitleLeft.text = titleLeft;
        txtTitleRight.text = titleRight;

        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(OnCloseButtonPressed);
    }

    private void OnCloseButtonPressed()
    {
        _actionClose?.Invoke();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed()
    {
        _actionOk?.Invoke();
        gameObject.SetActive(false);
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        object[] convert = (object[])data;
        Action actionDo = (Action)convert[0];
        Action actionSaveUserData = (Action)convert[1];
        string message = (string)convert[2];
        string title = (string)convert[3];
        int serverCurrentLevel = (int)convert[4];

        int serverCoin = (int)convert[5];
        int serverTotalSkin = (int)convert[6];
        string titleRight = (string)convert[7];
        string titleLeft = (string)convert[8];
        bool isBackup = (bool)convert[9];
        Action actionFuncLogin = (Action)convert[10];


        if (isBackup)
        {
            Initialized(() =>
                {
                    actionSaveUserData?.Invoke();
                    actionFuncLogin?.Invoke();
                },
                null,
                message,
                title,
                serverCurrentLevel,
                serverCoin,
                serverTotalSkin,
                titleRight,
                titleLeft,
                true);
        }
        else
        {
            Initialized(() =>
                {
                    actionDo?.Invoke();
                    actionFuncLogin?.Invoke();
                },
                null,
                message,
                title,
                serverCurrentLevel,
                serverCoin,
                serverTotalSkin,
                titleRight,
                titleLeft,
                false);
        }
    }

    public void Show() { BeforeShow(); }
}