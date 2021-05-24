using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopup : Popup
{
    [SerializeField] private GameObject purchaseButton;

    private void Awake()
    {
        purchaseButton.SetActive(false);
    }

    private void Start()
    {
#if UNITY_IOS
        purchaseButton.SetActive(true);
#endif
    }
}
