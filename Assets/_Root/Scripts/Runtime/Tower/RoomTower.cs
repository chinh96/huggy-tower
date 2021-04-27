using System.Collections.Generic;
using System.Linq;

namespace Lance.TowerWar.LevelBase
{
    using UnityEngine;
    using Common;

    public class RoomTower : MonoBehaviour
    {
        public RectTransform spawnPoint;
        [ReadOnly] public List<Unit> units = new List<Unit>();

        private void Start() { UpdateUnitCollection(); }

        public void UpdateUnitCollection()
        {
            units = GetComponentsInChildren<Unit>().ToList();
            
        }
    }
}