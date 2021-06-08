using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

using UnityEngine;

public class RoomTower : MonoBehaviour
{
    public Collider2D floor;
    public RectTransform spawnPoint;
    public Image selectedObject;

    public Sprite canSelectSprite;
    public Sprite cantSelectSprite;
    public List<Unit> units = new List<Unit>();
    public List<Item> items = new List<Item>();

    private void Start() { UpdateUnitCollection(); }

    public void UpdateUnitCollection()
    {
        units = GetComponentsInChildren<Unit>().ToList();
        items = GetComponentsInChildren<Item>().ToList();
    }

    public bool IsClearEnemyInRoom()
    {
        foreach (var unit in units)
        {
            if (unit.State != EUnitState.Invalid && unit.Type == EUnitType.Enemy) return false; // room not cleared
        }

        return true;
    }

    public void UpdateStatusSelectRoom(bool flagActive, bool flagSelect = false)
    {
        if (selectedObject != null)
        {
            selectedObject.gameObject.SetActive(flagActive);
            if (flagActive)
            {
                selectedObject.sprite = flagSelect ? canSelectSprite : cantSelectSprite;
            }
        }
    }

    public bool IsContaintPrincess()
    {
        foreach (var unit in units)
        {
            if (unit.State != EUnitState.Invalid && unit.Type == EUnitType.Princess) return true;
        }

        return false;
    }

    public bool IsContaintItem()
    {
        foreach (var unit in items)
        {
            if (unit.State != EUnitState.Invalid && (unit.Type == EUnitType.Item || unit.Type == EUnitType.Gem)) return true;
        }

        return false;
    }

    public bool IsRoomHaveUnitNotInvalid()
    {
        bool flag = false;
        foreach (var unit in units)
        {
            if (unit.State != EUnitState.Invalid && unit.Type != EUnitType.Hero) flag = true;
        }

        foreach (var item in items)
        {
            if (item.State != EUnitState.Invalid) flag = true;
        }

        return flag;
    }
}