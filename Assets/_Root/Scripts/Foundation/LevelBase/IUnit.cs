using Lance.TowerWar.Unit;

namespace Lance.TowerWar.LevelBase
{
    public interface IUnit
    {
        EUnitState State { get; set; }
        
        EUnitType Type { get; }
        
        /// <summary>
        /// object active
        /// </summary>
        void DarknessRise();

        /// <summary>
        /// object deactive
        /// </summary>
        void LightReturn();
    }
}