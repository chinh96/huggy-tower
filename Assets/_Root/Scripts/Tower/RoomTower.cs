using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class RoomTower : MonoBehaviour
{
    public Collider2D floor;
    public RectTransform spawnPoint; // spawn player at this point
    public Image selectedObject; // mark to this room is selecting

    public Sprite canSelectSprite;
    public Sprite cantSelectSprite;
    public List<Unit> units = new List<Unit>();
    public List<Item> items = new List<Item>();

    private bool isMousePress;

    private void Awake()
    {
        selectedObject?.gameObject.transform.SetAsFirstSibling();
        UpdateUnitCollection();
    }
    // private void Start() { UpdateUnitCollection(); }

    private void OnMouseDown()
    {
        isMousePress = true;
        UpdateStatusSelectRoom(true, true);
    }

    private void OnMouseEnter()
    {
        isMousePress = true;
        UpdateStatusSelectRoom(true, true);
    }

    private void OnMouseExit()
    {
        isMousePress = false;
        UpdateStatusSelectRoom(false, true);
    }

    private void OnMouseUp()
    {
        if (Data.CurrentLevel > 1 && GameController.Instance.Player.Turn == ETurn.Drag && GameController.Instance.LeanTouch.enabled)
        {
            bool hasPlayer = units.Find(unit => unit as Player) != null;
            var itemLock = items.Find(item => item as ItemLock);
            bool hasItemLock = itemLock != null && (itemLock.State == EUnitState.Invalid);
            if (isMousePress && !hasPlayer && !hasItemLock)
            {
                UpdateStatusSelectRoom(false, true);
                GameController.Instance.Player.FlashToSlot(this);
            }
        }
    }

    public void UpdateUnitCollection()
    {
        units = GetComponentsInChildren<Unit>().ToList();
        items = GetComponentsInChildren<Item>().ToList();
    }

    public void ChangeEnemyAnchorsAndPosition()
    {
        foreach (var enemy in units)
        {
            if(enemy as Princess != null && (enemy as Princess).LockObj != null){
                Debug.Log("Princess Lock!!");
                RectTransform uitransform = enemy.GetComponent<RectTransform>();
                uitransform.anchorMin = new Vector2(0.5f, 0);
                uitransform.anchorMax = new Vector2(0.5f, 0);
                uitransform.pivot = Vector2.zero;

                uitransform.anchoredPosition = new Vector3(uitransform.anchoredPosition.x, 35f, 0);
            }
            else if (enemy as EnemySpider == null && enemy as EnemySpider2 == null)
            {
                RectTransform uitransform = enemy.GetComponent<RectTransform>();
                uitransform.anchorMin = new Vector2(0.5f, 0);
                uitransform.anchorMax = new Vector2(0.5f, 0);
                uitransform.pivot = Vector2.zero;

                uitransform.anchoredPosition = new Vector3(uitransform.anchoredPosition.x, 0.7f, 0);
            }
        }
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
        if (selectedObject != null && GameController.Instance.Player.Turn == ETurn.Drag)
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

    public bool IsRoomHaveUnitNotInvalid() // equivalent with "Does room have any valid Unit?"
    {
        bool flag = false;
        foreach (var unit in units)
        {
            if (unit.State != EUnitState.Invalid && unit.Type != EUnitType.Hero) flag = true;
        }

        foreach (var item in items)
        {
            if (item.EquipType == ItemType.Lock && item.State == EUnitState.Invalid) return false;
            if (item.State != EUnitState.Invalid) flag = true;
        }

        return flag;
    }
}