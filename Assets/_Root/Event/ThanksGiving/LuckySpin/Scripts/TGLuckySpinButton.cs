using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGLuckySpinButton : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(TGDatas.IsInTG);
    }

    public void OnClick()
    {
        PopupController.Instance.Show<TGLuckySpinPopup>();
    }
}
