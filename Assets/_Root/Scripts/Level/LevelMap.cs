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
        if (DurationMoveCamera == 0 && !hasNewVisitTower)
        {
            GameController.Instance.SetEnableLeanTouch(true);
        }
    }

    public void ChangeToNewVisitTower()
    {
        visitTower.ChangeToHomeTower();
        indexVisitTower++;
        MoveToNewVisitTower();
    }

    public void MoveCameraVertical(RoomTower slot)
    {
        if (DurationMoveCamera > 0)
        {
            GameController.Instance.SetEnableLeanTouch(false);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, slot.transform.position.y - 5, Camera.main.transform.position.z);
            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
            {
                Camera.main.transform.DOMoveY(GameController.Instance.positionCameraOrigin.y, DurationMoveCamera).SetEase(Ease.Linear).OnComplete(() =>
                {
                    DOTween.Sequence().AppendInterval(2).AppendCallback(() =>
                    {
                        GameController.Instance.SetEnableLeanTouch(true);
                    });
                });
            });
        }
    }

    public void MoveCameraHorizontal()
    {
        if (hasNewVisitTower)
        {
            GameController.Instance.SetEnableLeanTouch(false);
            float endValue = (visitTowers[indexVisitTower].transform.position.x + visitTowers[indexVisitTower + 1].transform.position.x) / 2;
            Camera.main.transform.position = new Vector3(endValue, Camera.main.transform.position.y, Camera.main.transform.position.z);
            Camera.main.transform.DOMoveX(GameController.Instance.positionCameraOrigin.x, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                GameController.Instance.SetEnableLeanTouch(true);
            });
        }
    }

    public void MoveToNewVisitTower()
    {
        // a position between 2 visitTowers.
        float endValue = (visitTowers[indexVisitTower].transform.position.x + visitTowers[indexVisitTower - 1].transform.position.x) / 2;
        Camera.main.transform.DOMoveX(endValue, 2).SetEase(Ease.Linear);
        // if(indexVisitTower == 1){
        //    float endSize = Camera.main.orthographicSize + 1.5f;
        //    Camera.main.DOOrthoSize(endSize, 1);
        // }
            
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