using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinTotal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinTotal;

    private void Awake()
    {
        UpdateCoinText();
    }

    private void Start()
    {
        EventController.CoinTotalChanged += UpdateCoinText;
    }

    private void UpdateCoinText()
    {
        coinTotal.text = Data.CoinTotal.ToString();
    }

    public void ShowShopPopup()
    {
        PopupController.Instance.Show<ShopPopup>();
    }
}
