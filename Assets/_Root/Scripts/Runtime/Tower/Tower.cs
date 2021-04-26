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
}
