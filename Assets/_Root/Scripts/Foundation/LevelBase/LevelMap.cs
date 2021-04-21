using System.Collections.Generic;
using System.Linq;

namespace Lance.TowerWar.LevelBase
{
    public class LevelMap : Unit
    {
        public List<IUnit> Units { get; private set; }

        public override void DarknessRise()
        {
            if (Units == null || Units.Count == 0) Units = GetComponentsInChildren<IUnit>().ToList();

            foreach (var unit in Units)
            {
                if ((LevelMap) unit != this) unit.DarknessRise();
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