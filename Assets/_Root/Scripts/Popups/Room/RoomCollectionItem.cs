using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomCollectionItem : MonoBehaviour
{
    [SerializeField] private RoomPopup _roomPopup;

    [SerializeField] private GameObject _activeRoom;
    [SerializeField] private GameObject _lockRoom;

    [SerializeField] private TextMeshProUGUI _roomTitle;
    [SerializeField] private GameObject _huggyImage;

    private RoomResources _roomResources;

    public RoomType GetRoomType()
    {
        return _roomResources.roomType;
    }
    public void Init(RoomResources roomResources, bool isActive)
    {
        _roomResources = roomResources;

        _roomTitle.SetText(_roomResources.roomType.ToString());
        if (_roomResources.roomType == RoomType.ProductionLine) _roomTitle.SetText("Production \n Line");
        _activeRoom.GetComponent<Image>().sprite = _roomResources.upgradedFrame;

        Reset(isActive);
    }


    public void Reset(bool isActive)
    {
        _activeRoom.SetActive(isActive);
        _lockRoom.SetActive(!isActive);

        _huggyImage.transform.SetParent(_activeRoom.transform);
        if (isActive) _roomTitle.transform.SetParent(_activeRoom.transform);
        else _roomTitle.transform.SetParent(_lockRoom.transform);

        if (ResourcesController.Factory.RoomCurrent.roomType == _roomResources.roomType) _huggyImage.SetActive(true);
        else _huggyImage.SetActive(false);
    }

    public void HideHuggyImage()
    {
        _huggyImage.SetActive(false);
    }
    public void OnClickToThisRoom()
    {
        if (!_roomPopup.GetRoomCurrentObject().GetComponent<Room>().IsUpgrading())
        {
            _roomPopup.GetRoomCurrentObject().GetComponent<Room>().Character.Play("Idle", true);
            _roomPopup.FirePaper.gameObject.SetActive(false);
            
            Data.RoomCurrent = _roomResources.roomType;
            _roomPopup.ChangeToAnotherRoom();
            _huggyImage.SetActive(true);
        }
    }
}
