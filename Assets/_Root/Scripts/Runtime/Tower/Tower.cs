namespace Lance.TowerWar.Unit
{
    using System.Collections.Generic;
    using System.Linq;
    using LevelBase;
    using UnityEngine;

    public class Tower : MonoBehaviour
    {
        public List<RoomTower> slots;

        private void Start() { slots = GetComponentsInChildren<RoomTower>().ToList(); }

        public void RefreshRoom()
        {
            foreach (var roomTower in slots)
            {
                roomTower.UpdateUnitCollection();
            }
        }

        public bool IsClearTower()
        {
            var flag = true;
            foreach (var slot in slots)
            {
                flag = slot.IsClearEnemyInRoom();
                if (!flag)
                {
                    break;
                }
            }

            return flag;
        }
    }
}