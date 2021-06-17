using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;

public class Tower : MonoBehaviour
{
    public ContentSizeFitter fitter;
    public List<RoomTower> slots;
    public ParticleSystem smoke;

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

    public void RemoveSlot(RoomTower roomTower, Action action = null, float duration = .5f)
    {
        slots.Remove(roomTower);
        var fitter = GameController.Instance.Root.LevelMap.visitTower.fitter;
        roomTower.transform.DOScale(Vector3.zero, duration).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            fitter.enabled = false;
            fitter.enabled = true;
        }).OnComplete(() =>
        {
            Destroy(roomTower.gameObject);
            action?.Invoke();
        });
    }

    public void AddSlot()
    {
        SoundController.Instance.PlayOnce(SoundType.TowerLevelUp);

        var newRoom = Instantiate(GameController.Instance.RoomPrefab, transform, false);
        slots.Add(newRoom);

        newRoom.transform.localScale = Vector3.zero;
        newRoom.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InQuad);

        smoke.Play();
    }

    public void RemoveAll(Action action)
    {
        if (slots.Count > 0)
        {
            smoke.Play();
            RemoveSlot(slots[slots.Count - 1], () => RemoveAll(action), .3f);
        }
        else
        {
            action?.Invoke();
        }
    }
}