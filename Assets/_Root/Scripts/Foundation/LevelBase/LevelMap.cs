using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelMap : MonoBehaviour
{
    public ELevelCondition condition;
    public HomeTower homeTower;
    public VisitTower visitTower;
    public int CurrentRealLevelIndex { get; private set; }
    public int CurrentFakeLevelIndex { get; private set; }
    public List<IUnit> Units { get; private set; }
    public Vector3 offset = new Vector3(0, -150, 0);

    private void Start()
    {
        homeTower = GetComponentInChildren<HomeTower>();
        visitTower = GetComponentInChildren<VisitTower>();

        homeTower.transform.localPosition += offset;
        visitTower.transform.localPosition += offset;
    }

    public void SetLevelLoaded(int realLevelIndex, int fakeLevelIndex)
    {
        CurrentRealLevelIndex = realLevelIndex;
        CurrentFakeLevelIndex = fakeLevelIndex;
    }

    public void ResetSelectVisitTower()
    {
        foreach (var room in visitTower.slots)
        {
            room.UpdateStatusSelectRoom(false);
        }
    }

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