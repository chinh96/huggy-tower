using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Room : MonoBehaviour
{
    [SerializeField] private RoomPopup _roomPopup;
    [SerializeField] private RoomType _roomType;
    // [SerializeField] private GameObject _spanner; => sau này tối ưu code sẽ dùng
    [SerializeField] private List<Furniture> _furnitureList;
    [SerializeField] private Transform _originalParentTransform;
    public RoomType RoomType {get => _roomType; set => _roomType = value;}

    public void ReturnOriginalPosition(){
        transform.SetParent(_originalParentTransform, false);
        transform.SetSiblingIndex(0); // avoiding overlay UI Button
    }

    public void Init(){
        RoomResources currentRoom = ResourcesController.Factory.RoomCurrent;
        foreach(var furniture in _furnitureList){
            furniture.Init(currentRoom, this, _roomPopup);
        }
    }

    public void Reset(){
        foreach(var furniture in _furnitureList){
            furniture.Reset();
        }
        if(ResourcesController.Factory.RoomCurrent.IsComplete){
            _roomPopup.OpenNewRoom();
        }
    }

    public void SetBackground(){
        foreach(var furniture in _furnitureList){
            furniture.SetBackground();
        }
    }

    public bool IsUpgrading(){
        bool isUpgrading = false;
        foreach(var furniture in _furnitureList){
            if(furniture.IsUpgrading){
                isUpgrading = true;
                break;
            }
        }
        return isUpgrading;
    }
}
