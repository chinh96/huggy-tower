using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingPopup : Popup
{
    [SerializeField] private GameObject purchaseButton;
    [SerializeField] private TextMeshProUGUI versionText;

    private void Awake()
    {
        purchaseButton.SetActive(false);
    }

    private void Start()
    {
        versionText.text = $"Version {Application.version}";
#if UNITY_IOS
        purchaseButton.SetActive(true);
#endif
    }
}
