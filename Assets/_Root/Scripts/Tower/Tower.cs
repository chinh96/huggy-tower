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
    public ParticleSystem explosion;
    public Image homeTower;
    public Image homeTowerFlag;

    private void Start()
    {
        slots = GetComponentsInChildren<RoomTower>().ToList();

        if (this as VisitTower)
        {
            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                NumberedUnits();
            });

            DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
            {
                GameController.Instance.Root.LevelMap.MoveCamera(slots[0]);
            });
        }
    }

    private void NumberedUnits()
    {
        int index = 0;
        slots.ForEach(item =>
        {
            item.units.ForEach(unit =>
            {
                unit.GetComponent<Canvas>().sortingOrder = index + 5;
                index++;
            });
        });
    }

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
        roomTower.transform.DOScale(Vector3.zero, duration).SetEase(Ease.Linear).OnUpdate(() =>
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
            PlayExplosion();
            RemoveSlot(slots[slots.Count - 1], () => RemoveAll(action), .4f);
        }
        else
        {
            action?.Invoke();
        }
    }

    public void ChangeToHomTower()
    {
        float duration = 2;
        homeTower.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        homeTower.DOColor(new Color(1, 1, 1, 1), duration);
        homeTowerFlag.DOColor(new Color(1, 1, 1, 1), duration);

        slots.ForEach(slot =>
        {
            slot.transform.Find("Slot1").GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), duration);
        });
    }

    public void PlayExplosion()
    {
        explosion.Stop();
        explosion.Play();
    }
}