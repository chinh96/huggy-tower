using System;
using Lance.TowerWar.LevelBase;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public RoomTower[] slots;

    private void Start()
    {
        slots = GetComponentsInChildren<RoomTower>();
    }

    public void RefreshRoom()
    {
        foreach (var roomTower in slots)
        {
            roomTower.UpdateUnitCollection();
        }
    }
}
