using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public ContentSizeFitter fitter;
    public List<RoomTower> slots;

    private void Start() { slots = GetComponentsInChildren<RoomTower>().ToList(); }

    public void RefreshRoom()
    {
        foreach (var roomTower in slots)
        {
            roomTower.UpdateUnitCollection();
        }
    }

    public bool IsClearTower()
    {
        var flag = true;
        foreach (var slot in slots)
        {
            flag = slot.IsClearEnemyInRoom();
            Debug.Log("flag " + flag);
            if (!flag)
            {
                break;
            }
        }

        return flag;
    }

    public RoomTower RoomContainPlayer(Player player)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].units.Contains(player))
            {
                return slots[i];
            }
        }

        return null;
    }
}