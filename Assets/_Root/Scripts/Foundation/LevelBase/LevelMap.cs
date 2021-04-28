using System;
using System.Collections.Generic;
using System.Linq;
using Lance.TowerWar.Unit;
using UnityEngine;

namespace Lance.TowerWar.LevelBase
{
    public class LevelMap : MonoBehaviour
    {
        public HomeTower homeTower;
        public VisitTower visitTower;

        public int CurrentLevelIndex { get; private set; }
        public List<IUnit> Units { get; private set; }

        private void Start()
        {
            homeTower = GetComponentInChildren<HomeTower>();
            visitTower = GetComponentInChildren<VisitTower>();
        }

        /// <summary>
        /// set hold current level
        /// </summary>
        /// <param name="levelIndex"></param>
        public void SetLevelLoaded(int levelIndex) { CurrentLevelIndex = levelIndex; }

        public void DarknessRise()
        {
            if (Units == null || Units.Count == 0) Units = GetComponentsInChildren<IUnit>().ToList();

            foreach (var unit in Units)
            {
                unit?.DarknessRise();
            }
        }

        public void LightReturn()
        {
            if (Units == null || Units.Count == 0) Units = GetComponentsInChildren<IUnit>().ToList();

            foreach (var unit in Units)
            {
                unit?.LightReturn();
            }
        }
    }
}