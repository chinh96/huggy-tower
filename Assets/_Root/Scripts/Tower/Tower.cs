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
    public Sprite towerJapan;
    public Image tower;
    public VerticalLayoutGroup verticalLayoutGroup;
    public GameObject flag;

    [Header("JAPAN")]
    public Image HomeTowerJapan;
    public List<GameObject> flagJapans;
    public List<Image> HomeTowerJapanFlags;

    [Header("SEA")]
    public Image HomeTowerSea;
    public Sprite towerSea;
    public List<GameObject> flagSeas;

    [Header("HALLOWEEN")]
    public Image HomeTowerHalloween;
    public Sprite towerHalloween;
    public List<GameObject> flagHalloweens;

    [Header("HOME FOOTAGE AND ROOFTOP")]
    public Image HomeRoofTopImg;
    public Image HomeFootTageImg;

    [Header("FOOTAGE AND ROOFTOP")]
    public GameObject FootTage;
    public GameObject Rooftop;

    private void Start()
    {
        int childCount = transform.childCount;
        Rooftop.transform.SetSiblingIndex(0);
        FootTage.transform.SetSiblingIndex(childCount - 1);
        if (this as VisitTower)
        {
            homeTower.transform.SetSiblingIndex(1);
            var enemyImg = GetComponent<Image>();
            // enemyImg.SetNativeSize();
            enemyImg.type = Image.Type.Simple;
            // GetComponent<RectTransform>().sizeDelta = new Vector2(570, 100);
            slots = GetComponentsInChildren<RoomTower>().ToList();
            slots.ForEach(slot =>
            {
                slot.GetComponent<Image>().SetNativeSize();
                slot.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(520, 289);
            });

            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                // NumberedUnits();
            });

            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                GameController.Instance.Root.LevelMap.MoveCameraVertical(slots[0]);
                GameController.Instance.Root.LevelMap.MoveCameraHorizontal();
            });
        }
        else if (this as HomeTower)
        {
            GetComponent<RectTransform>().localPosition = new Vector2(-350,GetComponent<RectTransform>().localPosition.y);
            // GetComponent<RectTransform>().sizeDelta = new Vector2(470, 100);
            slots = GetComponentsInChildren<RoomTower>().ToList();
            slots.ForEach(slot =>
            {
                slot.GetComponent<Image>().SetNativeSize();
                // slot.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(444, 366);
            });

            var homeImg = GetComponent<Image>();
            homeImg.type = Image.Type.Simple;
            homeImg.SetNativeSize();

            // slots = GetComponentsInChildren<RoomTower>().ToList();
            // slots[0].GetComponent<Image>().SetNativeSize();
        }

        if (GameController.Instance.IsJapanBackground)
        {
            ChangeToJapanTower();
        }
        else if (GameController.Instance.IsSeaBackground)
        {
            ChangeToSeaTower();
        }
        else if (GameController.Instance.IsHalloweenBackground)
        {
            ChangeToHalloween();
        }
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        slots = GetComponentsInChildren<RoomTower>().ToList();
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            slot.GetComponent<Image>().SetNativeSize();
            slot.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(520, 289);
        }
    }
    private void NumberedUnits()
    {
        int index = 0;
        slots.ForEach(item =>
        {
            item.units.ForEach(unit =>
            {
                int offset = unit as EnemyKraken1 ? 30 : 5;
                unit.GetComponent<Canvas>().sortingOrder = index + offset;
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
        //newRoom.GetComponent<Image>().SetNativeSize();
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

    public void ChangeToHomeTower()
    {
        float duration = 2;

        // change to white color after 2 seconds.
        if (GameController.Instance.IsJapanBackground)
        {
            HomeTowerJapan.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            HomeTowerJapan.DOColor(new Color(1, 1, 1, 1), duration);

            HomeTowerJapanFlags.ForEach(homeTowerFlag =>
            {
                homeTowerFlag.DOColor(new Color(1, 1, 1, 1), duration);
            });
        }
        else if (GameController.Instance.IsSeaBackground)
        {
            HomeTowerSea.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            HomeTowerSea.DOColor(new Color(1, 1, 1, 1), duration);
        }
        else if (GameController.Instance.IsHalloweenBackground)
        {
            HomeTowerHalloween.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            HomeTowerHalloween.DOColor(new Color(1, 1, 1, 1), duration);
        }
        else
        {
            homeTower.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            homeTower.DOColor(new Color(1, 1, 1, 1), duration);
            homeTowerFlag.DOColor(new Color(1, 1, 1, 1), duration);
        }
        // DOTween.Sequence().AppendInterval(duration / 2).OnComplete(() =>
        // {
        //     tower.DOColor(new Color(0, 0, 0, 0), duration / 2);

        // });
        ChangeItemsToHome(duration);
    }
    private void ChangeItemsToHome(float duration)
    {
        slots.ForEach(slot =>
        {
            var slot1 = slot.transform.Find("Slot1");
            slot1.gameObject.SetActive(true);
            slot1.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), duration);
            slot1.transform.SetSiblingIndex(0);
            DOTween.Sequence().AppendInterval(duration / 2).OnComplete(() =>
            {
                slot.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), duration / 2);
            });
        });

        HomeFootTageImg.DOColor(new Color(1, 1, 1, 1), duration);
        DOTween.Sequence().AppendInterval(duration / 2).OnComplete(() =>
        {
            FootTage.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), duration / 2);

        });

        HomeRoofTopImg.DOColor(new Color(1, 1, 1, 1), duration);
        DOTween.Sequence().AppendInterval(duration / 2).OnComplete(() =>
        {
            Rooftop.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), duration / 2);
        });
    }

    public void PlayExplosion()
    {
        explosion.Stop();
        explosion.Play();
    }

    public virtual void ChangeToJapanTower()
    {
        tower.sprite = towerJapan;
        verticalLayoutGroup.padding.left = -10;
        verticalLayoutGroup.padding.top = 270;
        flag.SetActive(false);
        flagJapans.ForEach(flagJapan =>
        {
            flagJapan.SetActive(true);
        });
    }

    public virtual void ChangeToSeaTower()
    {
        tower.sprite = towerSea;
        if (this is VisitTower)
        {
            verticalLayoutGroup.padding.left = -10;
        }
        else
        {
            verticalLayoutGroup.padding.left = 10;
        }
        verticalLayoutGroup.padding.top = 500;
        flag.SetActive(false);
        flagSeas.ForEach(flagSea =>
        {
            flagSea.SetActive(true);
        });
    }

    public virtual void ChangeToHalloween()
    {
        tower.sprite = towerHalloween;
        verticalLayoutGroup.padding.left = -10;
        verticalLayoutGroup.padding.top = 270;
        flag.SetActive(false);
        flagHalloweens.ForEach(flagHalloween =>
        {
            flagHalloween.SetActive(true);
        });
    }
}