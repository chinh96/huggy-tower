using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;

public class LevelText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI level;

    private void Awake()
    {
        ChangeLevel();
        EventController.CurrentLevelChanged = ChangeLevel;
    }

    private void Start()
    {
    }

    public void ChangeLevel()
    {
        level.text = $"Level {Data.CurrentLevel + 1}";
        var localizationParamsManager = level.GetComponent<LocalizationParamsManager>();
        if(localizationParamsManager!= null) localizationParamsManager.SetParameterValue("VALUE", (Data.CurrentLevel + 1).ToString(), true);
    }

    public void ChangeLevelMinusOne(){
        level.text = $"Level {Data.CurrentLevel}";
        var localizationParamsManager = level.GetComponent<LocalizationParamsManager>();
        if(localizationParamsManager!= null) localizationParamsManager.SetParameterValue("VALUE", (Data.CurrentLevel).ToString(), true);
    }

    public void LevelBoss()
    {
        level.text = "BOSS";
    }
}
