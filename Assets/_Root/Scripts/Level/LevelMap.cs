using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class LevelMap : MonoBehaviour
{
    public ELevelCondition condition;
    public HomeTower homeTower;
    public VisitTower visitTower => visitTowers[indexVisitTower];
    public int CurrentRealLevelIndex { get; private set; }
    public int CurrentFakeLevelIndex { get; private set; }
    public List<IUnit> Units { get; private set; }
    public float DurationMoveCamera;
    public bool hasNewVisitTower => indexVisitTower + 1 < visitTowers.Length;

    private VisitTower[] visitTowers;
    private int indexVisitTower;

    private void Start()
    {
        homeTower = GetComponentInChildren<HomeTower>();
        visitTowers = GetComponentsInChildren<VisitTower>();
    }

    public void ChangeToNewVisitTower()
    {
        visitTower.ChangeToHomTower();
        indexVisitTower++;
        MoveCameraHorizontal();
    }

    public void MoveCameraVertical(RoomTower slot)
    {
        if (DurationMoveCamera > 0)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, slot.transform.position.y - 5, 0);
            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
            {
                Camera.main.transform.DOMoveY(0, DurationMoveCamera).SetEase(Ease.Linear).OnComplete(() =>
                {
                    GameController.Instance.ShowFighterOverlay();
                });
            });
        }
    }

    public void MoveCameraHorizontal()
    {
        float endValue = (visitTowers[indexVisitTower].transform.position.x + visitTowers[indexVisitTower - 1].transform.position.x) / 2;
        Camera.main.transform.DOMoveX(endValue, 2).SetEase(Ease.Linear);
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