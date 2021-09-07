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
    }

    private void Start()
    {
        EventController.CurrentLevelChanged = ChangeLevel;
    }

    public void ChangeLevel()
    {
        //level.text = $"Level {Data.CurrentLevel + 1}";
        level.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", (Data.CurrentLevel + 1).ToString(), true);
    }
}
