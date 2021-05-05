using System.Collections.Generic;
using System.Linq;
using Lance.TowerWar.Unit;

namespace Lance.TowerWar.LevelBase
{
    using UnityEngine;
    using Common;

    public class RoomTower : MonoBehaviour
    {
        public Collider2D floor;
        public RectTransform spawnPoint;
        [ReadOnly] public List<Unit> units = new List<Unit>();

        private void Start() { UpdateUnitCollection(); }

        public void UpdateUnitCollection() { units = GetComponentsInChildren<Unit>().ToList(); }

        /// <summary>
        /// return true if enemy in room Cleared
        /// </summary>
        /// <returns></returns>
        public bool IsClearEnemyInRoom()
        {
            var flag = true; // room cleared
            foreach (var unit in units)
            {
                if (unit.State != EUnitState.Invalid && unit.Type == EUnitType.Enemy)
                {
                    flag = false; // room not cleared
                    break;
                }
            }

            return flag;
        }
    }
}