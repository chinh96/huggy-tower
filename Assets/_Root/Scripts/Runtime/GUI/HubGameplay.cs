using TMPro;

namespace Lance.TowerWar.UI
{
    using UnityEngine;

    public class HubGameplay : MonoBehaviour
    {
        public TextMeshProUGUI txtCurrentLevel;

        public void UpdateDislayCurrentLevel(int level) { txtCurrentLevel.text = $"Level {level + 1}"; }
    }
}