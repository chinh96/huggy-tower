using Lance.TowerWar.Unit;

public interface IUnit
{
    UnityEngine.GameObject ThisGameObject { get; }
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