using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
public class PopupSelectLanguage : Popup
{
    [SerializeField] private Button btnOk;
    [SerializeField] private Button btnClose;
    [SerializeField] private ToggleGroup languageGroup;

    private Toggle[] tgLanguage;
    private Toggle selectLanguage;
    private Toggle currentLanguage;
    public void Initialize()
    {
        btnOk.onClick.RemoveListener(SelectLanguage);
        btnOk.onClick.AddListener(SelectLanguage);

        btnClose.onClick.RemoveListener(Back);
        btnClose.onClick.AddListener(Back);

        CheckLanguage();
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        Initialize();
    }

    private void CheckLanguage() 
    {
        tgLanguage = languageGroup.GetComponentsInChildren<Toggle>();
        tgLanguage[0].isOn = true;

        foreach (var tg in tgLanguage)
        {
            if (tg.GetComponent<SetLanguage>().IsSelect)
            {
                currentLanguage = tg;
                tg.isOn = true;
            }
        }
    }

    private void Update()
    {
        foreach (var tg in tgLanguage)
        {
            if (tg.isOn)
            {
                if(selectLanguage != tg) 
                {
                    selectLanguage = tg;
                    selectLanguage.GetComponent<SetLanguage>().ApplyLanguage();
                }
            }
        }
    }

    private void SelectLanguage()
    {
        currentLanguage = selectLanguage;
        currentLanguage.GetComponent<SetLanguage>().ApplyLanguage();
        Back();
    }

    private void Back()
    {
        if(currentLanguage != selectLanguage)
        {
            currentLanguage.GetComponent<SetLanguage>().ApplyLanguage();
        }
        Close();
    }
}
