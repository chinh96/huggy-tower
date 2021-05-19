using System;
using TMPro;
using UnityEngine;
using Lance.TowerWar;
using UnityEngine.UI;

public class HubGameplay : MonoBehaviour
{
    public TextMeshProUGUI txtQuest;
    public Button btnReplay;
    public Button btnSkip;

    public void UpdateDislayCurrentLevel(int level, ELevelCondition condition)
    {
        var str = "";
        switch (condition)
        {
            case ELevelCondition.KillAll:
                str = "kill all enemy";
                break;
            case ELevelCondition.CollectChest:
                str = "open chest";
                break;
            case ELevelCondition.SavePrincess:
                str = "save the princess";
                break;
            case ELevelCondition.CollectGold:
                str = "collect golds";
                break;
        }

        txtQuest.text = $"Level {level + 1}: {str.ToUpper()}";
    }

    public void AddListenerReplay(Action action)
    {
        btnReplay.onClick.RemoveAllListeners();
        btnReplay.onClick.AddListener(() => action?.Invoke());
    }

    public void AddListenerSkip(Action action)
    {
        btnSkip.onClick.RemoveAllListeners();
        btnSkip.onClick.AddListener(() => action?.Invoke());
    }
}