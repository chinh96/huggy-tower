using System;
using TMPro;
using UnityEngine.UI;

namespace Lance.TowerWar.UI
{
    using UnityEngine;

    public class HubGameplay : MonoBehaviour
    {
        public TextMeshProUGUI txtQuest;
        public Button btnReplay;

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
            }

            txtQuest.text = $"Level {level + 1}: {str.ToUpper()}";
        }

        public void AddListenerReplay(Action action)
        {
            btnReplay.onClick.RemoveAllListeners();
            btnReplay.onClick.AddListener(() => action?.Invoke());
        }
    }
}