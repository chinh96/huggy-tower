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
        [ReadOnly] public List<Item> items = new List<Item>();

        private void Start() { UpdateUnitCollection(); }

        public void UpdateUnitCollection()
        {
            units = GetComponentsInChildren<Unit>().ToList();
            items = GetComponentsInChildren<Item>().ToList();
        }

        /// <summary>
        /// return true if enemy in room Cleared
        /// </summary>
        /// <returns></returns>
        public bool IsClearEnemyInRoom()
        {
            foreach (var unit in units)
            {
                if (unit.State != EUnitState.Invalid && unit.Type == EUnitType.Enemy)
                {
                    return false; // room not cleared
                }
            }

            return true;
        }

        public bool IsContaintItem()
        {
            foreach (var unit in items)
            {
                if (unit.State != EUnitState.Invalid && unit.Type == EUnitType.Item)
                {
                    return true;
                }
            }

            return false;
        }
    }
}