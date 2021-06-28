using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class LevelMap : MonoBehaviour
{
    public ELevelCondition condition;
    public HomeTower homeTower;
    public VisitTower visitTower;
    public int CurrentRealLevelIndex { get; private set; }
    public int CurrentFakeLevelIndex { get; private set; }
    public List<IUnit> Units { get; private set; }
    public float DurationMoveCamera;

    private void Start()
    {
        homeTower = GetComponentInChildren<HomeTower>();
        visitTower = GetComponentInChildren<VisitTower>();
    }

    public void MoveCamera(RoomTower slot)
    {
        if (DurationMoveCamera > 0)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, slot.transform.position.y - 5, Camera.main.transform.position.z);
            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
            {
                Camera.main.transform.DOMoveY(0, DurationMoveCamera).SetEase(Ease.Linear).OnComplete(() =>
                {
                    GameController.Instance.ShowFighterOverlay();
                });
            });
        }
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