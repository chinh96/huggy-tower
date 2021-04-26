using System;
using System.Collections.Generic;
using System.Linq;
using Lance.TowerWar.Unit;

namespace Lance.TowerWar.LevelBase
{
    public class LevelMap : Unit
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

        public override void DarknessRise()
        {
            if (Units == null || Units.Count == 0) Units = GetComponentsInChildren<IUnit>().ToList();

            foreach (var unit in Units)
            {
                var levelMap = unit as LevelMap;
                if (levelMap != null && levelMap != this)
                {
                    unit.DarknessRise();
                }
            }
        }

        public override void LightReturn()
        {
            if (Units == null || Units.Count == 0) Units = GetComponentsInChildren<IUnit>().ToList();

            foreach (var unit in Units)
            {
                if ((LevelMap) unit != this) unit.LightReturn();
            }
        }
    }
}